using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace GUI
{
    public class Database
    {
        private string cs = @"server=localhost;userid=root;
            password=root;database=cinema";
        private MySqlConnection conn;
        private MySqlCommand cmd;
        MySqlDataReader rdr;
        public Database()
        {
            cmd = new MySqlCommand();
            rdr = null;
        }
        public void Connect()
        {
            try
            {
                conn = new MySqlConnection(cs);
                conn.Open();
            }
            catch (MySqlException ex)
            {
                System.Diagnostics.Debug.WriteLine("Error: {0}", ex.ToString());
            }

        }


        public void AddReservationToDabase(String data, int idFilmu, int liczbaBiletow)
        {
            cmd.Connection = conn;
            cmd.CommandText = "INSERT INTO rezerwacja(data,idFilmu, liczbaBiletow) VALUES(@data, @idFilmu, @liczbaBiletow);";
            cmd.Prepare();
            cmd.Parameters.AddWithValue("@data", data);
            cmd.Parameters.AddWithValue("@idFilmu", idFilmu);
            cmd.Parameters.AddWithValue("@liczbaBiletow", liczbaBiletow);
            cmd.ExecuteNonQuery();
        }

        public String GetFilm(int id)
        {
            String tytul = "";
            cmd.Connection = conn;
            String stm = "SELECT tytulFilmu FROM film WHERE idFilmu = @id;";
            cmd = new MySqlCommand(stm, conn);
            cmd.Prepare();
            cmd.Parameters.AddWithValue("@id", id);
            rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                tytul = rdr.GetString(0);
            }
            rdr.Close();
            return tytul;
        }

        public LinkedList<String> SelectAllFilms()
        {
            LinkedList<String> allFilms = new LinkedList<string>();
            cmd.Connection = conn;
            String stm = "SELECT tytulFilmu, godzina FROM film;";
            cmd = new MySqlCommand(stm, conn);
            cmd.Prepare();
            rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                allFilms.AddLast(rdr.GetString(0) + ", " + rdr.GetString(1)+":00");
            }
            rdr.Close();
            return allFilms;
        }

        public void AddFilmToDabase(String tytul, String godzina, String data)
        {
            cmd.Connection = conn;
            cmd.CommandText = "INSERT INTO film(tytulFilmu,godzina,data) VALUES(@Tytul, @godzina, @data);";
            cmd.Prepare();
            cmd.Parameters.AddWithValue("@Tytul", tytul);
            cmd.Parameters.AddWithValue("@godzina", godzina);
            cmd.Parameters.AddWithValue("@data", data);
            cmd.ExecuteNonQuery();
        }

    }
}
