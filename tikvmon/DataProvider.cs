using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Dapper;
using MySql.Data.MySqlClient;

namespace tikvmon
{
    public class DataProvider
    {
        // ssh -L 4000:localhost:4000 mysql-svr
        string cnStr;
        public DataProvider(string cnStr) {
            this.cnStr = cnStr;
        }
        MySqlConnection GetConnection() {
            return new MySqlConnection(cnStr);
        }
/*
INFORMATION_SCHEMA.TIKV_STORE_STATUS
+-------------------+-------------+------+------+---------+-------+
| Field             | Type        | Null | Key  | Default | Extra |
+-------------------+-------------+------+------+---------+-------+
| STORE_ID          | bigint(21)  | YES  |      | NULL    |       |
| ADDRESS           | varchar(64) | YES  |      | NULL    |       |
| STORE_STATE       | bigint(21)  | YES  |      | NULL    |       |
| STORE_STATE_NAME  | varchar(64) | YES  |      | NULL    |       |
| LABEL             | json        | YES  |      | NULL    |       |
| VERSION           | varchar(64) | YES  |      | NULL    |       |
| CAPACITY          | varchar(64) | YES  |      | NULL    |       |
| AVAILABLE         | varchar(64) | YES  |      | NULL    |       |
| LEADER_COUNT      | bigint(21)  | YES  |      | NULL    |       |
| LEADER_WEIGHT     | double      | YES  |      | NULL    |       |
| LEADER_SCORE      | double      | YES  |      | NULL    |       |
| LEADER_SIZE       | bigint(21)  | YES  |      | NULL    |       |
| REGION_COUNT      | bigint(21)  | YES  |      | NULL    |       |
| REGION_WEIGHT     | double      | YES  |      | NULL    |       |
| REGION_SCORE      | double      | YES  |      | NULL    |       |
| REGION_SIZE       | bigint(21)  | YES  |      | NULL    |       |
| START_TS          | datetime    | YES  |      | NULL    |       |
| LAST_HEARTBEAT_TS | datetime    | YES  |      | NULL    |       |
| UPTIME            | varchar(64) | YES  |      | NULL    |       |
+-------------------+-------------+------+------+---------+-------+
*/
        public class TiKVStoreStatus {
            public long STORE_ID {get;set;}
            public string ADDRESS {get;set;}
            public long STORE_STATE {get;set;}
            public string STORE_STATE_NAME {get;set;}
            public string LABEL {get;set;}
            public string VERSION {get;set;}
            public string CAPACITY {get;set;}
            public string AVAILABLE {get;set;}
            public long LEADER_COUNT {get;set;}
            public double LEADER_WEIGHT {get;set;}
            public double LEADER_SCORE {get;set;}
            public long LEADER_SIZE {get;set;}
            public long REGION_COUNT {get;set;}
            public double REGION_WEIGHT {get;set;}
            public double REGION_SCORE {get;set;}
            public long REGION_SIZE {get;set;}
            public DateTime START_TS {get;set;}
            public DateTime LAST_HEARTBEAT_TS {get;set;}
            public string UPTIME {get;set;}
            
            public StoreInfo ToStoreInfo() {
                return new StoreInfo {
                    Id = (int)STORE_ID,
                    Kind = LABEL != null && LABEL.Contains("tiflash") ? "TiFlash" : "TiKV",
                    Port = ADDRESS.Split(':').Last(),
                    LeaderCount = (int)LEADER_COUNT,
                    LeaderWeight = (int)LEADER_WEIGHT,
                    LeaderScore = (int)LEADER_SCORE,
                    LeaderSize = (int)LEADER_SIZE,
                    RegionCount = (int)REGION_COUNT,
                    RegionWeight = (int)REGION_WEIGHT,
                    RegionScore = (int)REGION_SCORE,
                    RegionSize = (int)REGION_SIZE,
                    UpTime = Regex.Replace(UPTIME, "[.]\\d+s$", "s")
                };
            }
        }

        public class TiKVRegionPeer {
            public int REGION_ID {get;set;}
            public int PEER_ID {get;set;}
            public int STORE_ID {get;set;}
            public int IS_LEARNER {get;set;}
            public int IS_LEADER {get;set;}

        }

        public List<TiKVStoreStatus> GetTiKVStoreStatus() {
            using (var cn = GetConnection()) {
                return cn.Query<TiKVStoreStatus>("SELECT * FROM INFORMATION_SCHEMA.TIKV_STORE_STATUS").ToList();
            }
        }
        public List<TiKVRegionPeer> GetTiKVRegionPeers() {
            using (var cn = GetConnection()) {
                return cn.Query<TiKVRegionPeer>("SELECT * FROM INFORMATION_SCHEMA.TIKV_REGION_PEERS").ToList();
            }
        }
                
        public class StoreInfo {
            public int Id {get;set;}
            public string Kind { get; set;}
            public string Port {get;set;}
            public int LeaderCount {get;set;}
            public int LeaderWeight {get;set;}
            public int LeaderScore {get;set;}
            public int LeaderSize {get;set;}
            public int RegionCount {get;set;}
            public int RegionWeight {get;set;}
            public int RegionScore {get;set;}
            public int RegionSize {get;set;}
            public string UpTime {get; set;}
        }


    }
}