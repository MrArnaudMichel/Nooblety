using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    public class SpatialHash
    {
        private const int MAX_SIZE = 32; // Must be POT
        private const int MIN_SIZE = 4;  // Must be POT
        
        private const int MIN_USE_SPATIAL_HASH = 64;
        private const int CANDIDATES_CAPACITY = MIN_USE_SPATIAL_HASH << 1;

        private static readonly HashSet<SpatialPointer> CandidatesHash = new HashSet<SpatialPointer>(CANDIDATES_CAPACITY);
        private static readonly List<SpatialPointer> CandidatesList = new List<SpatialPointer>(CANDIDATES_CAPACITY);

        private static readonly Vector3[] Bounds = new Vector3[4];

        // MEMBERS: -------------------------------------------------------------------------------

        [NonSerialized] private readonly Dictionary<int, SpatialItem> m_Items;
        [NonSerialized] private readonly Dictionary<ClusterKey, HashSet<int>> m_Clusters;
        
        [NonSerialized] private int m_UpdateFrame = -1;

        // CONSTRUCTOR: ---------------------------------------------------------------------------

        public SpatialHash()
        {
            this.m_Items = new Dictionary<int, SpatialItem>();
            this.m_Clusters = new Dictionary<ClusterKey, HashSet<int>>();
        }

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public void Insert(ISpatialHash spatialHash)
        {
            if (this.m_Items.ContainsKey(spatialHash.UniqueCode)) return;

            int uniqueCode = spatialHash.UniqueCode;
            SpatialItem itemData = new SpatialItem(spatialHash);
            this.m_Items.Add(uniqueCode, itemData);

            int clusterSize = MAX_SIZE;
            while (clusterSize >= MIN_SIZE)
            {
                Vector3Int clusterHash = ClusterKey.Hash(clusterSize, itemData.Position);
                ClusterKey clusterKey = new ClusterKey(clusterSize, clusterHash);

                if (this.m_Clusters.TryGetValue(clusterKey, out HashSet<int> content))
                {
                    content.Add(uniqueCode);
                }
                else
                {
                    this.m_Clusters.Add(clusterKey, new HashSet<int> { uniqueCode });
                }
                
                clusterSize >>= 1;
            }
        }

        public void Remove(ISpatialHash spatialHash)
        {
            bool contains = this.m_Items.TryGetValue(
                spatialHash.UniqueCode,
                out SpatialItem item
            );
            
            if (!contains) return;
            
            int clusterSize = MAX_SIZE;
            while (clusterSize >= MIN_SIZE)
            {
                Vector3Int clusterHash = ClusterKey.Hash(clusterSize, item.Position);
                ClusterKey clusterKey = new ClusterKey(clusterSize, clusterHash);

                this.m_Clusters[clusterKey].Remove(item.UniqueCode);
                clusterSize >>= 1;
            }
            
            this.m_Items.Remove(spatialHash.UniqueCode);
        }

        public bool Contains(ISpatialHash spatialHash)
        {
            return this.m_Items.ContainsKey(spatialHash.UniqueCode);
        }

        public void Find(Vector3 point, float radius, List<ISpatialHash> results, ISpatialHash except = null)
        {
            if (ApplicationManager.IsExiting) return;

            if (this.m_Items.Count < MIN_USE_SPATIAL_HASH)
            {
                CandidatesList.Clear();
                results.Clear();
                
                foreach (KeyValuePair<int, SpatialItem> entry in this.m_Items)
                {
                    if (entry.Value.Instance == null || entry.Value.Instance == except) continue;
                    
                    float distance = Vector3.Distance(point, entry.Value.Instance.Position);
                    if (distance > radius) continue;

                    SpatialPointer value = new SpatialPointer(entry.Key, distance);
                    CandidatesList.Add(value);
                }
                
                CandidatesList.Sort(SortByDistance);
                if (results.Capacity < CandidatesList.Count)
                {
                    results.Capacity = CandidatesList.Count;
                }
                
                foreach (SpatialPointer candidate in CandidatesList)
                {
                    int uniqueCode = candidate.UniqueCode;
                    results.Add(this.m_Items[uniqueCode].Instance);
                }

                return;
            }

            this.Update();

            Bounds[0] = new Vector3(point.x - radius, point.y + radius, point.z - radius);
            Bounds[1] = new Vector3(point.x + radius, point.y + radius, point.z - radius);
            Bounds[2] = new Vector3(point.x - radius, point.y - radius, point.z - radius);
            Bounds[3] = new Vector3(point.x - radius, point.y - radius, point.z + radius);

            CandidatesHash.Clear();
            CandidatesList.Clear();
            results.Clear();
            
            this.GetCandidates(MAX_SIZE, point, except?.UniqueCode ?? 0);
            
            CandidatesList.AddRange(CandidatesHash);
            CandidatesList.Sort(SortByDistance);
                
            foreach (SpatialPointer candidate in CandidatesList)
            {
                int uniqueCode = candidate.UniqueCode;
                results.Add(this.m_Items[uniqueCode].Instance);
            }
        }

        private void GetCandidates(int clusterSize, Vector3 point, int exceptUniqueCode)
        {
            Vector3Int cellA = ClusterKey.Hash(clusterSize, Bounds[0]); // (-1,  1,  1)
            Vector3Int cellB = ClusterKey.Hash(clusterSize, Bounds[1]); // ( 1,  1,  1)
            Vector3Int cellC = ClusterKey.Hash(clusterSize, Bounds[2]); // (-1, -1,  1)
            Vector3Int cellD = ClusterKey.Hash(clusterSize, Bounds[3]); // (-1,  1, -1)
                    
            for (int x = cellA.x; x <= cellB.x; ++x)
            {
                for (int y = cellC.y; y <= cellA.y; ++y)
                {
                    for (int z = cellC.z; z <= cellD.z; ++z)
                    {
                        Vector3Int cellHash = new Vector3Int(x, y, z);
                        ClusterKey clusterKey = new ClusterKey(clusterSize, cellHash);

                        bool clusterExists = this.m_Clusters.TryGetValue(
                            clusterKey,
                            out HashSet<int> children
                        );
                        
                        if (!clusterExists || children.Count == 0) continue;
                        if (children.Count < MIN_USE_SPATIAL_HASH || clusterSize <= MIN_SIZE)
                        {
                            foreach (int childUniqueCode in children)
                            {
                                if (childUniqueCode == exceptUniqueCode) continue;

                                Vector3 childPoint = this.m_Items[childUniqueCode].Position;
                                float distance = Vector3.Distance(childPoint, point);
                                
                                CandidatesHash.Add(new SpatialPointer(childUniqueCode, distance));
                            }
                        }
                        else
                        {
                            this.GetCandidates(clusterSize >> 1, point, exceptUniqueCode);
                        }
                    }
                }
            }
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------

        private void Update()
        {
            if (this.m_UpdateFrame == Time.frameCount) return;

            foreach (KeyValuePair<int, SpatialItem> itemEntry in this.m_Items)
            {
                if (itemEntry.Value.Instance == null) continue;
                
                Vector3 currentPosition = itemEntry.Value.Position;
                Vector3 newPosition = itemEntry.Value.Instance.Position;

                if (currentPosition == newPosition) continue;

                int clusterSize = MAX_SIZE;
                while (clusterSize >= MIN_SIZE)
                {
                    Vector3Int currentClusterHash = ClusterKey.Hash(clusterSize, currentPosition);
                    Vector3Int newClusterHash = ClusterKey.Hash(clusterSize, newPosition);
                    
                    bool differentHash = 
                        currentClusterHash.x != newClusterHash.x ||
                        currentClusterHash.y != newClusterHash.y ||
                        currentClusterHash.z != newClusterHash.z;
                    
                    if (differentHash)
                    {
                        ClusterKey currentClusterKey = new ClusterKey(clusterSize, currentClusterHash);
                        ClusterKey newClusterKey = new ClusterKey(clusterSize, newClusterHash);

                        HashSet<int> currentClusterList = this.m_Clusters[currentClusterKey]; 
                        currentClusterList.Remove(itemEntry.Key);

                        if (currentClusterList.Count == 0)
                        {
                            this.m_Clusters.Remove(currentClusterKey);
                        }

                        if (!this.m_Clusters.TryGetValue(newClusterKey, out HashSet<int> newClusterList))
                        {
                            newClusterList = new HashSet<int>();
                            this.m_Clusters.Add(newClusterKey, newClusterList);
                        }
                        
                        newClusterList.Add(itemEntry.Key);
                    }
                    
                    clusterSize >>= 1;
                }

                itemEntry.Value.Position = newPosition;
            }
            
            this.m_UpdateFrame = Time.frameCount;
        }
        
        private static int SortByDistance(SpatialPointer a, SpatialPointer b)
        {
            return a.Distance.CompareTo(b.Distance);
        }
    }
}