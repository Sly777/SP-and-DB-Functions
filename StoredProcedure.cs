using System;
using System.Collections;
using System.Data;
using System.Configuration;
using System.Data.SqlClient;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

// Stored Procedure and DB Action Classes
// http://developerarea.blogspot.com
//

// STOREDPROCEDURE CLASS
/// <summary>
/// Need this to run DB Functions
/// </summary>
public class StoredProcedure
{
    private string sProcName;
    private ArrayList sParams = new ArrayList();

    /// <summary>
    /// Add new parameter to SQL Query
    /// </summary>
    /// <param name="pName">Parameter Name</param>
    /// <param name="pDataType">Parameter Type</param>
    /// <param name="pValue">Parameter Value</param>
    public void SetParam(string pName, SqlDbType pDataType, string pValue)
    {
        ParamData pData = new ParamData(pName, pDataType, pValue);
        sParams.Add(pData);
    }

    public ArrayList GetParams()
    {
        return sParams ?? null;
    }

    /// <summary>
    /// Get / Set SQL Procedure Name to run SQL Query
    /// </summary>
    /// <param name="pName">Parameter Name</param>
    /// <param name="pDataType">Parameter Type</param>
    /// <param name="pValue">Parameter Value</param>
    public string ProcName
    {
        get { return sProcName; }
        set { sProcName = value; }
    }
}

public struct ParamData
{
    public string pName, pValue;
    public SqlDbType pDataType;

    public ParamData(string pName, SqlDbType pDataType, string pValue)
    {
        this.pName = pName;
        this.pDataType = pDataType;
        this.pValue = pValue;
    }
}

// DBACTION CLASS
/// <summary>
/// Database Functions
/// </summary>
public class dbActions
{
    /// <summary>
    /// Execute Sql Query with Parameters
    /// </summary>
    /// <param name="sData">StoredProcedure Class</param>
    /// <returns></returns>
    public static DataSet SqlQueryDataSet(StoredProcedure sData)
    {
        DataSet sqlDataView = new DataSet();

        try
        {
            SqlConnection conn = new SqlConnection(connect());
            SqlCommand sqlcomm = new SqlCommand(sData.ProcName, conn);
            sqlcomm.CommandType = CommandType.StoredProcedure;

            int i = 0;
            IEnumerator myEnumerator = sData.GetParams().GetEnumerator();
            while (myEnumerator.MoveNext())
            {
                ParamData pData = (ParamData)myEnumerator.Current;
                sqlcomm.Parameters.Add(pData.pName, pData.pDataType);
                sqlcomm.Parameters[i].Value = pData.pValue;
                i = i + 1;
            }

            SqlDataAdapter sqlAdapterView = new SqlDataAdapter(sqlcomm);

            sqlAdapterView.Fill(sqlDataView);

            return sqlDataView;
        }
        catch (Exception ex)
        {
            HttpContext.Current.Response.Write(ex);
            HttpContext.Current.Response.End();

            return sqlDataView;
        }
    }

    /// <summary>
    /// Execute Sql Query with Storeprocedure Name only (without Parameters)
    /// </summary>
    /// <param name="sData">StoredProcedure Class</param>
    /// <returns></returns>
    public static DataSet SqlTextDataSet(StoredProcedure sData)
    {
        DataSet sqlDataView = new DataSet();

        try
        {
            SqlConnection conn = new SqlConnection(connect());
            SqlCommand sqlcomm = new SqlCommand(sData.ProcName, conn);
            sqlcomm.CommandType = CommandType.Text;

            SqlDataAdapter sqlAdapterView = new SqlDataAdapter(sqlcomm);

            sqlAdapterView.Fill(sqlDataView);

            return sqlDataView;
        }
        catch (Exception ex)
        {
            HttpContext.Current.Response.Write(ex);
            HttpContext.Current.Response.End();

            return sqlDataView;
        }
    }

    /// <summary>
    /// Execute Sql Query with text only (without Storeprocedure Class)
    /// </summary>
    /// <param name="data">SQL Query</param>
    /// <returns></returns>
    public static void updateSqlnonQuery(string data)
    {
        try
        {
            SqlConnection conn = new SqlConnection(connect());
            SqlCommand sql = new SqlCommand(data, conn);
            conn.Open();
            sql.ExecuteNonQuery();
            conn.Close();
        }
        catch (Exception ex)
        {
            HttpContext.Current.Response.Write(ex);
            HttpContext.Current.Response.End();
        }

    }

    private static string connect()
    {
        return ConfigurationManager.ConnectionStrings["CONNECTION_NAME"].ToString();
    }

    private readonly HttpContext thisPage = HttpContext.Current;
}
