using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Xml;

namespace Type_TableToModelList
{

    /// <summary>
    /// 数据库操作公共类。
    /// </summary>
    public class SQLDatabase
    {
        private string serverName, dbName, userID, passWord;// encryptKey;
        private string sqlConnectionString;
        private SqlConnection conn;

        /// <summary>
        /// 配置文件中取连接数据库的字符串
        /// </summary>
        public SQLDatabase()
        {
            // 初始化数据库连接字符串
            this.sqlConnectionString = ConfigurationManager.ConnectionStrings["SzNet.Conn"].ToString();
        }
        /// <param name="sqlconnect"></param>
        public SQLDatabase(string sqlconnect)
        {
            this.sqlConnectionString = sqlconnect;
        }

        public SQLDatabase(string serverName, string dbName, string userID, string passWord)
        {
            this.serverName = serverName;
            this.dbName = dbName;
            this.userID = userID;
            this.passWord = passWord;
            //this.encryptKey = ConfigurationManager.AppSettings["EncryptKey"].Trim();
            //this.userID = ClsEncrypt.Encrypt(userID, this.encryptKey);
            //this.passWord = ClsEncrypt.Encrypt(passWord, this.encryptKey);
            this.sqlConnectionString = "user id=" + this.userID + ";password=" + this.passWord + ";initial catalog=" + this.dbName + ";data source=" + this.serverName + ";Connect Timeout=60";
        }

        /// <summary>
        /// 是否使用日志
        /// </summary>
        /// <returns>是与否</returns>
        private bool UseLog()
        {
            if (ConfigurationManager.AppSettings["Log"] != null)
            {
                string use = ConfigurationManager.AppSettings["Log"].Trim();

                if (use.ToUpper() == "TRUE")
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 打开数据库连接。
        /// </summary>
        private void Open()
        {
            if (this.conn == null)
            {
                this.conn = new SqlConnection(this.sqlConnectionString);
                this.conn.Open();
            }
        }

        /// <summary>
        /// 关闭数据库连接。
        /// </summary>
        public void Close()
        {
            if (this.conn != null)
            {
                this.conn.Close();
                this.Dispose();
            }
        }

        /// <summary>
        /// Release resources.
        /// 释放资源。
        /// </summary>
        public void Dispose()
        {
            if (this.conn != null)
            {
                this.conn.Dispose();
                this.conn = null;
            }
        }

        /// <summary>
        /// 测试连接
        /// </summary>
        /// <returns>成功与否</returns>
        public bool TestConnection()
        {
            try
            {
                this.conn = null;
                this.Open();
            }
            catch (Exception ex)
            {
               
                return false;
            }
            finally
            {
                this.Close();
            }
            return true;
        }

        /// <summary>
        /// 测试连接
        /// </summary>
        /// <param name="server">服务器地址</param>
        /// <param name="database">数据库名</param>
        /// <param name="uid">用户名</param>
        /// <param name="password">密码</param>
        /// <returns>成功与否</returns>
        public bool TestConnection(string serverName, string dbName, string userID, string passWord)
        {
            try
            {
                this.conn = null;
                this.sqlConnectionString = "user id=" + userID + ";password=" + passWord + ";initial catalog=" + dbName + ";data source=" + serverName + ";Connect Timeout=60";
                this.Open();
            }
            catch (Exception ex)
            {
                return false;
            }
            finally
            {
                this.Close();
            }
            return true;
        }



        /// <summary>
        /// 执行存储过程 eg:
        /// RunProc("upProcedureName");			// run the stored procedure
        /// </summary>
        /// <param name="procName">存储过程名</param>
        /// <returns>成功是否</returns>
        public bool RunProc(string procName)
        {
            try
            {
                SqlHelper.ExecuteNonQuery(this.sqlConnectionString, CommandType.StoredProcedure, procName);
            }
            catch (Exception ex)
            {
                return false;
            }
            finally
            {
                this.Close();
            }
            return true;
        }

        /// <summary>
        /// 执行存储过程 eg:
        /// RunProc("upProcedureName", prams);			// run the stored procedure
        /// strVlaue = (string) prams[index].Value;     // get the output param value 获得输出参数的值
        /// </summary>
        /// <param name="procName">存储过程名</param>
        /// <param name="prams">参数</param>
        /// <returns>成功是否</returns>
        public bool RunProc(string procName, SqlParameter[] prams)
        {
            try
            {
                SqlHelper.ExecuteNonQuery(this.sqlConnectionString, CommandType.StoredProcedure, procName, prams);
            }
            catch (Exception ex)
            {
                return false;
            }
            finally
            {
                this.Close();
            }
            return true;
        }

        /// <summary>
        /// 通过查询指定的StoredProcedure语句来获得一个返回值，带parms
        /// </summary>
        /// <param name="procName">存储过程名</param>
        /// <param name="prams">参数</param>
        /// <returns>返回值</returns>
        public string RunProcValue(string procName, SqlParameter[] prams)
        {
            try
            {
                return SqlHelper.ExecuteScalar(this.sqlConnectionString, CommandType.StoredProcedure, procName, prams).ToString();
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                this.Close();
            }
        }

        /// <summary>
        /// 通过查询指定的StoredProcedure语句来获得一个返回值，不带parms
        /// </summary>
        /// <param name="procName">存储过程名</param>
        /// <returns>返回值</returns>
        public string RunProcValue(string procName)
        {
            try
            {
                return SqlHelper.ExecuteScalar(this.sqlConnectionString, CommandType.StoredProcedure, procName).ToString();
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                this.Close();
            }
        }
        /// <summary>
        /// 通过查询指定的StoredProcedure语句来获得一个返回表，不带parms
        /// </summary>
        /// <param name="procName">存储过程名</param>
        /// <returns>DataTable</returns>
        public DataTable RunProcDataTable(string procName)
        {

            try
            {
                return SqlHelper.ExecuteDataset(this.sqlConnectionString, CommandType.StoredProcedure, procName).Tables[0];
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                this.Close();
            }
        }

        /// <summary>
        /// 通过查询指定的StoredProcedure语句来获得一个返回表，带parms
        /// </summary>
        /// <param name="sql">存储过程名</param>
        /// <param name="prams">参数</param>
        /// <returns>DataTable</returns>
        public DataTable RunProcDataTable(string procName, SqlParameter[] parms)
        {

            try
            {
                return SqlHelper.ExecuteDataset(this.sqlConnectionString, CommandType.StoredProcedure, procName, parms).Tables[0];
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                this.Close();
            }
        }

        /// <summary>
        /// 通过查询指定的StoredProcedure语句来获得一个返回表，带parms
        /// </summary>
        /// <param name="sql">存储过程名</param>
        /// <param name="prams">参数</param>
        /// <returns>DataTable</returns>
        public DataSet RunProcDataSet(string procName, SqlParameter[] parms)
        {

            try
            {
                return SqlHelper.ExecuteDataset(this.sqlConnectionString, CommandType.StoredProcedure, procName, parms);
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                this.Close();
            }
        }

        /// <summary>
        /// 通过查询指定的StoredProcedure语句来获得一个返回表，带parms
        /// </summary>
        /// <param name="sql">存储过程名</param>
        /// <param name="prams">参数</param>
        /// <returns>DataReader</returns>
        public SqlDataReader RunProcDataReader(string procName, SqlParameter[] parms)
        {

            try
            {
                return SqlHelper.ExecuteReader(this.sqlConnectionString, CommandType.StoredProcedure, procName, parms);
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                this.Close();
            }
        }

        /// <summary>
        /// 执行一个无返回的sql语句
        /// </summary>				
        /// <param name="sql">sql语句</param>
        /// <returns>成功是否</returns>
        public bool QueryExec(string sql)
        {
            try
            {
                SqlHelper.ExecuteNonQuery(this.sqlConnectionString, CommandType.Text, sql);
            }
            catch (Exception ex)
            {
                return false;
            }
            finally
            {
                this.Close();
            }
            return true;
        }

        /// <summary>
        /// 执行一个无返回的sql语句，带parameters
        /// </summary>				
        /// <param name="sql">sql语句</param>
        /// <param name="parms">参数</param>
        /// <returns>成功是否</returns>
        public bool QueryExec(string sql, SqlParameter[] parms)
        {
            try
            {
                SqlHelper.ExecuteNonQuery(this.sqlConnectionString, CommandType.Text, sql, parms);
            }
            catch (Exception ex)
            {
                return false;
            }
            finally
            {
                this.Close();
            }
            return true;
        }

        #region LW修改

        /// <summary>
        /// 执行一个返回影响行数的sql语句
        /// </summary>				
        /// <param name="sql">sql语句</param>
        /// <returns>成功是否</returns>
        public int QueryExecValue(string sql)
        {
            try
            {
                sql += "; SELECT @@rowcount AS [rowcount];";
                return SqlHelper.ExecuteNonQuery(this.sqlConnectionString, CommandType.Text, sql);
            }
            catch (Exception ex)
            {
                return 0;
            }
            finally
            {
                this.Close();
            }
        }

        /// <summary>
        /// 执行一个返回影响行数的sql语句，带parameters
        /// </summary>				
        /// <param name="sql">sql语句</param>
        /// <param name="parms">参数</param>
        /// <returns>成功是否</returns>
        public int QueryExecValue(string sql, SqlParameter[] parms)
        {
            try
            {
                sql += "; SELECT @@rowcount AS [rowcount];";
                return SqlHelper.ExecuteNonQuery(this.sqlConnectionString, CommandType.Text, sql, parms);
            }
            catch (Exception ex)
            {
                return 0;
            }
            finally
            {
                this.Close();
            }
        }
        #endregion


        #region WL修改

        /// <summary>
        /// 判断记录是否存在
        /// </summary>				
        /// <param name="sql">sql语句</param>
        /// <returns>成功是否</returns>
        public bool Exists(string sql)
        {
            try
            {
                return SqlHelper.ExecuteScalar(this.sqlConnectionString, CommandType.Text, sql).ToString() != "0";
            }
            catch (Exception ex)
            {
                return false;
            }
            finally
            {
                this.Close();
            }
        }

        /// <summary>
        /// 判断记录是否存在，带parameters
        /// </summary>				
        /// <param name="sql">sql语句</param>
        /// <param name="parms">参数</param>
        /// <returns>成功是否</returns>
        public bool Exists(string sql, SqlParameter[] parms)
        {
            try
            {
                return SqlHelper.ExecuteScalar(this.sqlConnectionString, CommandType.Text, sql, parms).ToString() != "0";
            }
            catch (Exception ex)
            {
                return false;
            }
            finally
            {
                this.Close();
            }
        }
        #endregion



        /// <summary>
        /// 执行一个插入记录操作，返回primary key
        /// </summary>				
        /// <param name="sql">insert sql语句</param>
        /// <returns>返回的primary key</returns>
        public string InsertExec(string sql)
        {
            sql += ";SELECT @@identity AS [@@IDENTITY];";
            try
            {
                return SqlHelper.ExecuteScalar(this.sqlConnectionString, CommandType.Text, sql).ToString();
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                this.Close();
            }
        }

        /// <summary>
        /// 执行一个插入记录操作，带parameters，返回primary key
        /// </summary>				
        /// <param name="sql">insert sql语句</param>
        /// <param name="parms">参数</param>
        /// <returns>返回的primary key</returns>
        public string InsertExec(string sql, SqlParameter[] prams)
        {
            sql += ";SELECT @@identity AS [@@IDENTITY];";
            try
            {
                return SqlHelper.ExecuteScalar(this.sqlConnectionString, CommandType.Text, sql, prams).ToString();
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                this.Close();
            }
        }

        /// <summary>
        /// 通过查询指定的SQL语句来获得一个返回值
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <returns>返回值</returns>
        public string QueryValue(string sql)
        {
            try
            {
                return SqlHelper.ExecuteScalar(this.sqlConnectionString, CommandType.Text, sql).ToString();
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                this.Close();
            }
        }

        /// <summary>
        /// 通过查询指定的SQL语句来获得一个返回值
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <returns>返回值</returns>
        public string QueryValue(string constr, string sql)
        {
            try
            {
                if (string.IsNullOrEmpty(constr))
                    return SqlHelper.ExecuteScalar(this.sqlConnectionString, CommandType.Text, sql).ToString();
                else
                    return SqlHelper.ExecuteScalar(constr, CommandType.Text, sql).ToString();
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                this.Close();
            }
        }

        /// <summary>
        /// 通过查询指定的SQL语句来获得一个返回值，带parms
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="parms">参数</param>
        /// <returns>返回值</returns>
        public string QueryValue(string sql, SqlParameter[] prams)
        {
            try
            {
                return SqlHelper.ExecuteScalar(this.sqlConnectionString, CommandType.Text, sql, prams).ToString();
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                this.Close();
            }
        }

        /// <summary>
        /// 通过查询指定的SQL语句来获得一个返回值，带parms
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="parms">参数</param>
        /// <returns>返回值</returns>
        public string QueryValue(string constr, string sql, SqlParameter[] prams)
        {
            try
            {
                if (string.IsNullOrEmpty(constr))
                    return SqlHelper.ExecuteScalar(this.sqlConnectionString, CommandType.Text, sql, prams).ToString();
                else
                    return SqlHelper.ExecuteScalar(constr, CommandType.Text, sql, prams).ToString();
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                this.Close();
            }
        }

        /// <summary>
        /// 通过查询指定的SQL语句来获得一个返回表
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <returns>表</returns>
        public DataTable QueryDataTable(string sql)
        {
            try
            {
                return SqlHelper.ExecuteDataset(this.sqlConnectionString, CommandType.Text, sql).Tables[0];
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                this.Close();
            }
        }

        /// <summary>
        /// 通过查询指定的SQL语句来获得一个返回表
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <returns>表</returns>
        public DataTable QueryDataTable(string constr, string sql)
        {
            try
            {
                if (string.IsNullOrEmpty(constr))
                    return SqlHelper.ExecuteDataset(this.sqlConnectionString, CommandType.Text, sql).Tables[0];
                else
                    return SqlHelper.ExecuteDataset(constr, CommandType.Text, sql).Tables[0];
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                this.Close();
            }
        }

        /// <summary>
        /// 通过查询指定的SQL语句来获得一个返回表，带parms
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="parms">参数</param>
        /// <returns>表</returns>
        public DataTable QueryDataTable(string sql, SqlParameter[] parms)
        {
            try
            {
                return SqlHelper.ExecuteDataset(this.sqlConnectionString, CommandType.Text, sql, parms).Tables[0];
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                this.Close();
            }
        }

        /// <summary>
        /// 通过查询指定的SQL语句来获得一个返回表，带parms
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="parms">参数</param>
        /// <returns>表</returns>
        public DataTable QueryDataTable(string constr, string sql, SqlParameter[] parms)
        {
            try
            {
                if (string.IsNullOrEmpty(constr))
                    return SqlHelper.ExecuteDataset(this.sqlConnectionString, CommandType.Text, sql, parms).Tables[0];
                else
                    return SqlHelper.ExecuteDataset(constr, CommandType.Text, sql, parms).Tables[0];
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                this.Close();
            }
        }

        /// <summary>
        /// 查询返回DATAREADER
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <returns>DATAREADER</returns>
        public SqlDataReader QueryDataReader(string sql)
        {
            try
            {
                return SqlHelper.ExecuteReader(this.sqlConnectionString, CommandType.Text, sql);
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                this.Close();
            }
        }

        /// <summary>
        /// 查询返回DATAREADER，带parms
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="parms">参数</param>
        /// <returns>DATAREADER</returns>
        public SqlDataReader QueryDataReader(string sql, SqlParameter[] parms)
        {
            try
            {
                return SqlHelper.ExecuteReader(this.sqlConnectionString, CommandType.Text, sql, parms);
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                this.Close();
            }
        }

        /// <summary>
        /// 查询返回DATASET
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <returns>DataSet对象</returns>
        public DataSet QueryDataSet(string sql)
        {
            try
            {
                return SqlHelper.ExecuteDataset(this.sqlConnectionString, CommandType.Text, sql);
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                this.Close();
            }
        }


        /// <summary>
        /// 查询返回DATASET，带parms
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="parms">参数</param>
        /// <returns>DataSet对象</returns>
        public DataSet QueryDataSet(string sql, SqlParameter[] parms)
        {
            try
            {
                return SqlHelper.ExecuteDataset(this.sqlConnectionString, CommandType.Text, sql, parms);
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                this.Close();
            }
        }


        /// <summary>
        /// 查询返回Xml
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <returns>XmlReader对象</returns>
        public XmlReader QueryXml(string sql)
        {
            try
            {
                return SqlHelper.ExecuteXmlReader(this.sqlConnectionString, CommandType.Text, sql);
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                this.Close();
            }
        }


        /// <summary>
        /// 查询返回Xml，带parms
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="parms">参数</param>
        /// <returns>XmlReader对象</returns>
        public XmlReader QueryXml(string sql, SqlParameter[] parms)
        {
            try
            {
                return SqlHelper.ExecuteXmlReader(this.sqlConnectionString, CommandType.Text, sql, parms);
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                this.Close();
            }
        }



        /// <summary>
        /// 更新Dataset。
        /// </summary>
        /// <param name="insertCommand">插入Command</param>
        /// <param name="deleteCommand">删除Command</param>
        /// <param name="updateCommand">更新Command</param>
        /// <param name="ds">Dataset</param>
        /// <param name="tableName">表名</param>
        public void UpdateDataset(SqlCommand insertCommand, SqlCommand deleteCommand, SqlCommand updateCommand, DataSet ds, string tableName)
        {
            try
            {
                SqlHelper.UpdateDataset(insertCommand, deleteCommand, updateCommand, ds, tableName);
            }
            catch (Exception ex)
            {
            }
        }

        /// <summary>
        /// 填充DataSet
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="ds">Dataset</param>
        /// <param name="tableNames">表名</param>
        /// <returns>成功是否</returns>
        public bool FillDataSet(string sql, DataSet ds, string[] tableNames)
        {
            try
            {
                SqlHelper.FillDataset(this.sqlConnectionString, CommandType.Text, sql, ds, tableNames);
            }
            catch (Exception ex)
            {
                return false;
            }
            finally
            {
                this.Close();
            }
            return true;
        }

        /// <summary>
        /// 填充DataSet，带parms
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="ds">Dataset</param>
        /// <param name="tableNames">表名</param>
        /// <param name="parms">参数</param>
        /// <returns>成功是否</returns>
        public bool FillDataSet(string sql, DataSet ds, string[] tableNames, SqlParameter[] parms)
        {
            try
            {
                SqlHelper.FillDataset(this.sqlConnectionString, CommandType.Text, sql, ds, tableNames, parms);
            }
            catch (Exception ex)
            {
                return false;
            }
            finally
            {
                this.Close();
            }
            return true;
        }

        /// <summary>
        /// Make input param.
        /// 包装输入参数。
        /// </summary>
        /// <param name="ParamName">参数名</param>
        /// <param name="DbType">参数类型</param>
        /// <param name="Size">参数容量</param>
        /// <param name="Value">参数值</param>
        /// <returns>新的Sql参数</returns>
        public SqlParameter MakeInParam(string ParamName, SqlDbType DbType, int Size, object Value)
        {
            return MakeParam(ParamName, DbType, Size, ParameterDirection.Input, Value);
        }

        /// <summary>
        /// Make input param.
        /// 包装输入参数。
        /// </summary>
        /// <param name="ParamName">参数名</param>
        /// <param name="DbType">参数类型</param>
        /// <param name="Size">参数容量</param>
        /// <param name="Value">参数值</param>
        /// <returns>新的Sql参数</returns>
        public SqlParameter MakeInParam(string ParamName, SqlDbType DbType, object Value)
        {
            return MakeParam(ParamName, DbType, 0, ParameterDirection.Input, Value);
        }

        /// <summary>
        /// Make output param.
        /// 包装输出参数。
        /// </summary>
        /// <param name="ParamName">参数名</param>
        /// <param name="DbType">参数类型</param>
        /// <param name="Size">参数容量</param>
        /// <returns>新的Sql参数</returns>
        public SqlParameter MakeOutParam(string ParamName, SqlDbType DbType, int Size)
        {
            return MakeParam(ParamName, DbType, Size, ParameterDirection.Output, null);
        }

        /// <summary>
        /// Make output param.
        /// 包装输出参数。
        /// </summary>
        /// <param name="ParamName">参数名</param>
        /// <param name="DbType">参数类型</param>
        /// <returns>新的Sql参数</returns>
        public SqlParameter MakeOutParam(string ParamName, SqlDbType DbType)
        {
            return MakeParam(ParamName, DbType, 0, ParameterDirection.Output, null);
        }

        /// <summary>
        /// Make output param.
        /// 包装输出参数。
        /// </summary>
        /// <param name="ParamName">参数名</param>
        /// <param name="DbType">参数类型</param>
        /// <param name="Size">参数容量</param>
        /// <param name="Value">参数值</param>
        /// <returns>新的Sql参数</returns>
        public SqlParameter MakeOutParam(string ParamName, SqlDbType DbType, int Size, object Value)
        {
            return MakeParam(ParamName, DbType, Size, ParameterDirection.Output, Value);
        }

        /// <summary>
        /// Make output param.
        /// 包装输出参数。
        /// </summary>
        /// <param name="ParamName">参数名</param>
        /// <param name="DbType">参数类型</param>
        /// <param name="Size">参数容量</param>
        /// <param name="Value">参数值</param>
        /// <returns>新的Sql参数</returns>
        public SqlParameter MakeOutParam(string ParamName, SqlDbType DbType, object Value)
        {
            return MakeParam(ParamName, DbType, 0, ParameterDirection.Output, Value);
        }

        /// <summary>
        /// Make stored procedure param.
        /// 包装Command参数。
        /// </summary>
        /// <param name="ParamName">参数名</param>
        /// <param name="DbType">参数类型</param>
        /// <param name="Size">参数容量</param>
        /// <param name="Direction">参数方向</param>
        /// <param name="Value">参数值</param>
        /// <returns>新的Sql参数</returns>
        public SqlParameter MakeParam(string ParamName, SqlDbType DbType, Int32 Size, ParameterDirection Direction, object Value)
        {
            SqlParameter param;

            if (Size > 0)
                param = new SqlParameter(ParamName, DbType, Size);
            else
                param = new SqlParameter(ParamName, DbType);

            param.Direction = Direction;
            if (!(Direction == ParameterDirection.Output && Value == null))
                param.Value = Value;
            else Value = param.Value;
            return param;
        }

        /// <summary>
        /// 创建command对象以便执行sql语句。
        /// </summary>
        /// <param name="sql">Sql Text.</param>		
        /// <returns>Command object.</returns>
        public SqlCommand CreateCommand(string sql, params string[] sourceColumns)
        {
            try
            {
                return SqlHelper.CreateCommand(this.sqlConnectionString, sql, sourceColumns);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public int ExecuteNonQueryTran(SqlTransaction tran,string sql, DataRow dr)
        {
          
                int rtn = SqlHelper.ExecuteNonQueryTypedParams(tran, sql, dr);
                return rtn;
           
         
        }


        /// <summary>
        /// 执行存储过程，返回Output输出参数值        
        /// </summary>
        /// <param name="storedProcName">存储过程名</param>
        /// <param name="parameters">存储过程参数</param>
        /// <returns>object</returns>
        public  object RunProcedure(string storedProcName, IDataParameter[] paramenters)
        {
            using (SqlConnection connection = new SqlConnection(sqlConnectionString))
            {
                connection.Open();
                SqlCommand command = BuildQueryCommand(connection, storedProcName, paramenters);
                command.ExecuteNonQuery();
                object obj = command.Parameters["@Output_Value"].Value; //@Output_Value和具体的存储过程参数对应
                if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
                {
                    return null;
                }
                else
                {
                    return obj;
                }
            }
        }

        /// <summary>
        /// 构建 SqlCommand 对象(用来返回一个结果集，而不是一个整数值)
        /// </summary>
        /// <param name="connection">数据库连接</param>
        /// <param name="storedProcName">存储过程名</param>
        /// <param name="parameters">存储过程参数</param>
        /// <returns>SqlCommand</returns>
        private static SqlCommand BuildQueryCommand(SqlConnection connection, string storedProcName, IDataParameter[] parameters)
        {
            SqlCommand command = new SqlCommand(storedProcName, connection);
            command.CommandType = CommandType.StoredProcedure;
            foreach (SqlParameter parameter in parameters)
            {
                if (parameter != null)
                {
                    // 检查未分配值的输出参数,将其分配以DBNull.Value.
                    if ((parameter.Direction == ParameterDirection.InputOutput || parameter.Direction == ParameterDirection.Input) &&
                        (parameter.Value == null))
                    {
                        parameter.Value = DBNull.Value;
                    }
                    command.Parameters.Add(parameter);
                }
            }

            return command;
        }

    }
}