using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Dieseltech
{
    public class Utility
    {
        static String constr = ConfigurationManager.ConnectionStrings["DieselTechcs"].ConnectionString.ToString();
        SqlConnection objConnection = new SqlConnection(constr);

        //Function to get Carrier Information in autocomplete
        public DataSet GetName(string prefix)

        {
            //SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["DieselTechcs"].ConnectionString);
            //string qry = "select CarrierName, MC#,AssignID, ContactName, Phonenumber from tblCarrier where CarrierName like '%'+@prefix+'%'  ";

            string qry = "select IsBlackList, ContactName2 as ContactNametwo, Phonenumber2 as Phonenumbertwo ,CarrierName, MC#,AssignID, ContactName, Phonenumber from tblCarrier where isdeleted = 0 and  (CarrierName like '%'+@prefix+'%'  ";
            qry += " or MC# like '%'+@prefix+'%' or  AssignID like '%'+@prefix+'%' or ContactName like '%'+@prefix+'%' or  Phonenumber  like '%'+@prefix+'%' )   ";

            SqlCommand com = new SqlCommand(qry, objConnection);

            com.Parameters.AddWithValue("@prefix", prefix);

            DataSet ds = new DataSet();

            SqlDataAdapter da = new SqlDataAdapter(com);

            da.Fill(ds);

            return ds;

        }




        public DataSet GetCityStateNameDetail(string prefix)

        {
            
            //string qry = "select CarrierName, MC#,AssignID, ContactName, Phonenumber from tblCarrier where CarrierName like '%'+@prefix+'%'  ";
            //qry += " or MC# like '%'+@prefix+'%' or  AssignID like '%'+@prefix+'%' or ContactName like '%'+@prefix+'%' or  Phonenumber  like '%'+@prefix+'%'    ";



            string qry = "SELECT top 5  ZipCode, CityName, StateCode FROM            tblStateCityData where CityName like '%'+@prefix+'%'  ";
            qry += " or StateCode like '%'+@prefix+'%' or  ZipCode like '%'+@prefix+'%' ";


            SqlCommand com = new SqlCommand(qry, objConnection);

            com.Parameters.AddWithValue("@prefix", prefix);

            DataSet ds = new DataSet();

            SqlDataAdapter da = new SqlDataAdapter(com);

            da.Fill(ds);

            return ds;

        }



        //Get Broker Function
        public DataSet GetBroker(string prefix)

        {
            //SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["DieselTechcs"].ConnectionString);
            //string qry = "select CarrierName, MC#,AssignID, ContactName, Phonenumber from tblCarrier where CarrierName like '%'+@prefix+'%'  ";

            string qry = "select AssignID,Name,MC,ContactName,Phone  from tblBroker where Name like '%'+@prefix+'%'  ";
            qry += " or MC like '%'+@prefix+'%' or  AssignID like '%'+@prefix+'%' or ContactName like '%'+@prefix+'%' or  Phone  like '%'+@prefix+'%'    ";

            SqlCommand com = new SqlCommand(qry, objConnection);

            com.Parameters.AddWithValue("@prefix", prefix);

            DataSet ds = new DataSet();

            SqlDataAdapter da = new SqlDataAdapter(com);

            da.Fill(ds);

            return ds;

        }


        //Get Broker Function
        public DataSet GetTruckList(string prefix)

        {
            
            string qry = "select  LD.LoadTypeName,SD.CityName,SD.StateCode,AvailableDate,DriverName,TruckNo,TrailerNo,D.Phone	  from tblTruck TR ";
            qry += " inner join tblDriver D ON TR.DriverId = D.DriverId inner join tblStateCityData SD on SD.ZipCode = TR.ZipCode Inner join tblLoadType LD ON LD.LoadTypeId = TR.TrailerTypeId   ";
            qry += " where TR.CarrierAssignId = ''+@prefix+''   ";
            //qry += " or D.Name like '%'+@prefix+'%' or  TR.TruckNo  like '%'+@prefix+'%' or  D.Phone  like '%'+@prefix+'%'   ";
      

            SqlCommand com = new SqlCommand(qry, objConnection);

            com.Parameters.AddWithValue("@prefix", prefix);

            DataSet ds = new DataSet();

            SqlDataAdapter da = new SqlDataAdapter(com);

            da.Fill(ds);

            return ds;

        }

        //Function to insert/update in db
        public String InsertUpdate(String qry)
        {
            Int32 result = 0;
            try
            {
                objConnection.Open();
                SqlCommand cmd = new SqlCommand(qry, objConnection);
                result=cmd.ExecuteNonQuery();
                objConnection.Close();
                return Convert.ToString(result);
            }
            catch (Exception ex)
            {
                
                return ex.Message;
            }
            finally
            {
                objConnection.Close();
               
            }

        }

        //Get datatable function 
        public DataTable GetDatatable(String qry)
        {
            objConnection.Open();
            SqlCommand cmd = new SqlCommand(qry, objConnection);
            SqlDataAdapter sda = new SqlDataAdapter(qry,objConnection);
            cmd.ExecuteNonQuery();
            DataTable dt = new DataTable();
            sda.Fill(dt);
            objConnection.Close();
            return dt;
        }

        //Function to get single value from DB
        public string  ExecuteScalar(string qry)
        {
            string   str = "";
            SqlCommand dbcmd = new SqlCommand(qry, objConnection);
            try
            {
                dbcmd.Connection.Open();
                //if (dbcmd.ExecuteScalar() != null)
                //{
                    str =  dbcmd.ExecuteScalar().ToString();
                //}


            }
            catch (System.Data.SqlClient.SqlException ex)
            {
                throw ex;
            }
            finally
            {
                dbcmd.Connection.Close();
            }

            return str;
        }

    }
}