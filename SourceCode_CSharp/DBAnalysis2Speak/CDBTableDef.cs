using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBAnalysis2Speak
{
    public class CDBTableDef
    {
        #region Inner structure

        public const string TableType_BaseTable = "BASE TABLE";
        public const string TableType_View = "VIEW";
        public const string TableType_SystemView = "SYSTEM VIEW";

		public enum EDbRow_Format
        {
			Fixed, 
			Dynamic, 
			Compressed, 
			Redundant, 
			Compact, 
			Paged

		}
        #endregion//Inner structure

		public string TABLE_CATALOG;// VARCHAR(64) NULL DEFAULT NULL COLLATE 'utf8_tolower_ci',
		public string TABLE_SCHEMA;// VARCHAR(64) NULL DEFAULT NULL COLLATE 'utf8_tolower_ci',
		public string TABLE_NAME;//  VARCHAR(64) NULL DEFAULT NULL COLLATE 'utf8_tolower_ci',
		public string TABLE_TYPE;//  ENUM('BASE TABLE','VIEW','SYSTEM VIEW') NOT NULL COLLATE 'utf8_bin',
		public string ENGINE;// VARCHAR(64) NULL DEFAULT NULL COLLATE 'utf8_general_ci',
		public short VERSION;// INT(2) NULL DEFAULT NULL,
		public EDbRow_Format ROW_FORMAT;//  ENUM('Fixed','Dynamic','Compressed','Redundant','Compact','Paged') NULL DEFAULT NULL COLLATE 'utf8_bin',
		public long TABLE_ROWS;//  BIGINT(21) UNSIGNED NULL DEFAULT NULL,
		public int AVG_ROW_LENGTH;//  BIGINT(21) UNSIGNED NULL DEFAULT NULL,
		public long DATA_LENGTH;// BIGINT(21) UNSIGNED NULL DEFAULT NULL,
		public long MAX_DATA_LENGTH;//  BIGINT(21) UNSIGNED NULL DEFAULT NULL,
		public long INDEX_LENGTH;// BIGINT(21) UNSIGNED NULL DEFAULT NULL,
		public long DATA_FREE;//  BIGINT(21) UNSIGNED NULL DEFAULT NULL,
		public long AUTO_INCREMENT;//  BIGINT(21) UNSIGNED NULL DEFAULT NULL,
		public int CREATE_TIME;// TIMESTAMP NOT NULL,
		public DateTime UPDATE_TIME;// DATETIME NULL DEFAULT NULL,
		public DateTime CHECK_TIME;// DATETIME NULL DEFAULT NULL,
		public string TABLE_COLLATION;//  VARCHAR(64) NULL DEFAULT NULL COLLATE 'utf8_general_ci',
		public string CHECKSUM;// BIGINT(21) NULL DEFAULT NULL,
		public string CREATE_OPTIONS;//  VARCHAR(256) NULL DEFAULT NULL COLLATE 'utf8_general_ci',
		public string TABLE_COMMENT;//  VARCHAR(256) NULL DEFAULT NULL COLLATE 'utf8_general_ci'
	}//class CDBTableDef
}//namespace
