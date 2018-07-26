using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Xml;

namespace Type_TableToModelList
{

    /// <summary>
    /// ���ݿ���������ࡣ
    /// </summary>
    public class SQLDatabase
    {
        private string serverName, dbName, userID, passWord;// encryptKey;
        private string sqlConnectionString;
        private SqlConnection conn;

        /// <summary>
        /// �����ļ���ȡ�������ݿ���ַ���
        /// </summary>
        public SQLDatabase()
        {
            // ��ʼ�����ݿ������ַ���
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
        /// �Ƿ�ʹ����־
        /// </summary>
        /// <returns>�����</returns>
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
        /// �����ݿ����ӡ�
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
        /// �ر����ݿ����ӡ�
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
        /// �ͷ���Դ��
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
        /// ��������
        /// </summary>
        /// <returns>�ɹ����</returns>
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
        /// ��������
        /// </summary>
        /// <param name="server">��������ַ</param>
        /// <param name="database">���ݿ���</param>
        /// <param name="uid">�û���</param>
        /// <param name="password">����</param>
        /// <returns>�ɹ����</returns>
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
        /// ִ�д洢���� eg:
        /// RunProc("upProcedureName");			// run the stored procedure
        /// </summary>
        /// <param name="procName">�洢������</param>
        /// <returns>�ɹ��Ƿ�</returns>
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
        /// ִ�д洢���� eg:
        /// RunProc("upProcedureName", prams);			// run the stored procedure
        /// strVlaue = (string) prams[index].Value;     // get the output param value ������������ֵ
        /// </summary>
        /// <param name="procName">�洢������</param>
        /// <param name="prams">����</param>
        /// <returns>�ɹ��Ƿ�</returns>
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
        /// ͨ����ѯָ����StoredProcedure��������һ������ֵ����parms
        /// </summary>
        /// <param name="procName">�洢������</param>
        /// <param name="prams">����</param>
        /// <returns>����ֵ</returns>
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
        /// ͨ����ѯָ����StoredProcedure��������һ������ֵ������parms
        /// </summary>
        /// <param name="procName">�洢������</param>
        /// <returns>����ֵ</returns>
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
        /// ͨ����ѯָ����StoredProcedure��������һ�����ر�����parms
        /// </summary>
        /// <param name="procName">�洢������</param>
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
        /// ͨ����ѯָ����StoredProcedure��������һ�����ر���parms
        /// </summary>
        /// <param name="sql">�洢������</param>
        /// <param name="prams">����</param>
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
        /// ͨ����ѯָ����StoredProcedure��������һ�����ر���parms
        /// </summary>
        /// <param name="sql">�洢������</param>
        /// <param name="prams">����</param>
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
        /// ͨ����ѯָ����StoredProcedure��������һ�����ر���parms
        /// </summary>
        /// <param name="sql">�洢������</param>
        /// <param name="prams">����</param>
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
        /// ִ��һ���޷��ص�sql���
        /// </summary>				
        /// <param name="sql">sql���</param>
        /// <returns>�ɹ��Ƿ�</returns>
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
        /// ִ��һ���޷��ص�sql��䣬��parameters
        /// </summary>				
        /// <param name="sql">sql���</param>
        /// <param name="parms">����</param>
        /// <returns>�ɹ��Ƿ�</returns>
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

        #region LW�޸�

        /// <summary>
        /// ִ��һ������Ӱ��������sql���
        /// </summary>				
        /// <param name="sql">sql���</param>
        /// <returns>�ɹ��Ƿ�</returns>
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
        /// ִ��һ������Ӱ��������sql��䣬��parameters
        /// </summary>				
        /// <param name="sql">sql���</param>
        /// <param name="parms">����</param>
        /// <returns>�ɹ��Ƿ�</returns>
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


        #region WL�޸�

        /// <summary>
        /// �жϼ�¼�Ƿ����
        /// </summary>				
        /// <param name="sql">sql���</param>
        /// <returns>�ɹ��Ƿ�</returns>
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
        /// �жϼ�¼�Ƿ���ڣ���parameters
        /// </summary>				
        /// <param name="sql">sql���</param>
        /// <param name="parms">����</param>
        /// <returns>�ɹ��Ƿ�</returns>
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
        /// ִ��һ�������¼����������primary key
        /// </summary>				
        /// <param name="sql">insert sql���</param>
        /// <returns>���ص�primary key</returns>
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
        /// ִ��һ�������¼��������parameters������primary key
        /// </summary>				
        /// <param name="sql">insert sql���</param>
        /// <param name="parms">����</param>
        /// <returns>���ص�primary key</returns>
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
        /// ͨ����ѯָ����SQL��������һ������ֵ
        /// </summary>
        /// <param name="sql">SQL���</param>
        /// <returns>����ֵ</returns>
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
        /// ͨ����ѯָ����SQL��������һ������ֵ
        /// </summary>
        /// <param name="sql">SQL���</param>
        /// <returns>����ֵ</returns>
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
        /// ͨ����ѯָ����SQL��������һ������ֵ����parms
        /// </summary>
        /// <param name="sql">SQL���</param>
        /// <param name="parms">����</param>
        /// <returns>����ֵ</returns>
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
        /// ͨ����ѯָ����SQL��������һ������ֵ����parms
        /// </summary>
        /// <param name="sql">SQL���</param>
        /// <param name="parms">����</param>
        /// <returns>����ֵ</returns>
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
        /// ͨ����ѯָ����SQL��������һ�����ر�
        /// </summary>
        /// <param name="sql">sql���</param>
        /// <returns>��</returns>
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
        /// ͨ����ѯָ����SQL��������һ�����ر�
        /// </summary>
        /// <param name="sql">sql���</param>
        /// <returns>��</returns>
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
        /// ͨ����ѯָ����SQL��������һ�����ر���parms
        /// </summary>
        /// <param name="sql">sql���</param>
        /// <param name="parms">����</param>
        /// <returns>��</returns>
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
        /// ͨ����ѯָ����SQL��������һ�����ر���parms
        /// </summary>
        /// <param name="sql">sql���</param>
        /// <param name="parms">����</param>
        /// <returns>��</returns>
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
        /// ��ѯ����DATAREADER
        /// </summary>
        /// <param name="sql">sql���</param>
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
        /// ��ѯ����DATAREADER����parms
        /// </summary>
        /// <param name="sql">sql���</param>
        /// <param name="parms">����</param>
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
        /// ��ѯ����DATASET
        /// </summary>
        /// <param name="sql">sql���</param>
        /// <returns>DataSet����</returns>
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
        /// ��ѯ����DATASET����parms
        /// </summary>
        /// <param name="sql">sql���</param>
        /// <param name="parms">����</param>
        /// <returns>DataSet����</returns>
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
        /// ��ѯ����Xml
        /// </summary>
        /// <param name="sql">sql���</param>
        /// <returns>XmlReader����</returns>
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
        /// ��ѯ����Xml����parms
        /// </summary>
        /// <param name="sql">sql���</param>
        /// <param name="parms">����</param>
        /// <returns>XmlReader����</returns>
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
        /// ����Dataset��
        /// </summary>
        /// <param name="insertCommand">����Command</param>
        /// <param name="deleteCommand">ɾ��Command</param>
        /// <param name="updateCommand">����Command</param>
        /// <param name="ds">Dataset</param>
        /// <param name="tableName">����</param>
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
        /// ���DataSet
        /// </summary>
        /// <param name="sql">sql���</param>
        /// <param name="ds">Dataset</param>
        /// <param name="tableNames">����</param>
        /// <returns>�ɹ��Ƿ�</returns>
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
        /// ���DataSet����parms
        /// </summary>
        /// <param name="sql">sql���</param>
        /// <param name="ds">Dataset</param>
        /// <param name="tableNames">����</param>
        /// <param name="parms">����</param>
        /// <returns>�ɹ��Ƿ�</returns>
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
        /// ��װ���������
        /// </summary>
        /// <param name="ParamName">������</param>
        /// <param name="DbType">��������</param>
        /// <param name="Size">��������</param>
        /// <param name="Value">����ֵ</param>
        /// <returns>�µ�Sql����</returns>
        public SqlParameter MakeInParam(string ParamName, SqlDbType DbType, int Size, object Value)
        {
            return MakeParam(ParamName, DbType, Size, ParameterDirection.Input, Value);
        }

        /// <summary>
        /// Make input param.
        /// ��װ���������
        /// </summary>
        /// <param name="ParamName">������</param>
        /// <param name="DbType">��������</param>
        /// <param name="Size">��������</param>
        /// <param name="Value">����ֵ</param>
        /// <returns>�µ�Sql����</returns>
        public SqlParameter MakeInParam(string ParamName, SqlDbType DbType, object Value)
        {
            return MakeParam(ParamName, DbType, 0, ParameterDirection.Input, Value);
        }

        /// <summary>
        /// Make output param.
        /// ��װ���������
        /// </summary>
        /// <param name="ParamName">������</param>
        /// <param name="DbType">��������</param>
        /// <param name="Size">��������</param>
        /// <returns>�µ�Sql����</returns>
        public SqlParameter MakeOutParam(string ParamName, SqlDbType DbType, int Size)
        {
            return MakeParam(ParamName, DbType, Size, ParameterDirection.Output, null);
        }

        /// <summary>
        /// Make output param.
        /// ��װ���������
        /// </summary>
        /// <param name="ParamName">������</param>
        /// <param name="DbType">��������</param>
        /// <returns>�µ�Sql����</returns>
        public SqlParameter MakeOutParam(string ParamName, SqlDbType DbType)
        {
            return MakeParam(ParamName, DbType, 0, ParameterDirection.Output, null);
        }

        /// <summary>
        /// Make output param.
        /// ��װ���������
        /// </summary>
        /// <param name="ParamName">������</param>
        /// <param name="DbType">��������</param>
        /// <param name="Size">��������</param>
        /// <param name="Value">����ֵ</param>
        /// <returns>�µ�Sql����</returns>
        public SqlParameter MakeOutParam(string ParamName, SqlDbType DbType, int Size, object Value)
        {
            return MakeParam(ParamName, DbType, Size, ParameterDirection.Output, Value);
        }

        /// <summary>
        /// Make output param.
        /// ��װ���������
        /// </summary>
        /// <param name="ParamName">������</param>
        /// <param name="DbType">��������</param>
        /// <param name="Size">��������</param>
        /// <param name="Value">����ֵ</param>
        /// <returns>�µ�Sql����</returns>
        public SqlParameter MakeOutParam(string ParamName, SqlDbType DbType, object Value)
        {
            return MakeParam(ParamName, DbType, 0, ParameterDirection.Output, Value);
        }

        /// <summary>
        /// Make stored procedure param.
        /// ��װCommand������
        /// </summary>
        /// <param name="ParamName">������</param>
        /// <param name="DbType">��������</param>
        /// <param name="Size">��������</param>
        /// <param name="Direction">��������</param>
        /// <param name="Value">����ֵ</param>
        /// <returns>�µ�Sql����</returns>
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
        /// ����command�����Ա�ִ��sql��䡣
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
        /// ִ�д洢���̣�����Output�������ֵ        
        /// </summary>
        /// <param name="storedProcName">�洢������</param>
        /// <param name="parameters">�洢���̲���</param>
        /// <returns>object</returns>
        public  object RunProcedure(string storedProcName, IDataParameter[] paramenters)
        {
            using (SqlConnection connection = new SqlConnection(sqlConnectionString))
            {
                connection.Open();
                SqlCommand command = BuildQueryCommand(connection, storedProcName, paramenters);
                command.ExecuteNonQuery();
                object obj = command.Parameters["@Output_Value"].Value; //@Output_Value�;���Ĵ洢���̲�����Ӧ
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
        /// ���� SqlCommand ����(��������һ���������������һ������ֵ)
        /// </summary>
        /// <param name="connection">���ݿ�����</param>
        /// <param name="storedProcName">�洢������</param>
        /// <param name="parameters">�洢���̲���</param>
        /// <returns>SqlCommand</returns>
        private static SqlCommand BuildQueryCommand(SqlConnection connection, string storedProcName, IDataParameter[] parameters)
        {
            SqlCommand command = new SqlCommand(storedProcName, connection);
            command.CommandType = CommandType.StoredProcedure;
            foreach (SqlParameter parameter in parameters)
            {
                if (parameter != null)
                {
                    // ���δ����ֵ���������,���������DBNull.Value.
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