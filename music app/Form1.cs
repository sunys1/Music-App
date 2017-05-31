
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.IO;


namespace music_app
{
    public partial class Form1 : Form
    {     
        public Form1()
        {
            InitializeComponent();
            axWindowsMediaPlayer1.settings.autoStart = true;
            Refresh();
            Refresh2();
            textBoxFileName.Enabled = false;
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;

            if (listBox1.SelectedItem == null|| comboBox1.SelectedItem == null || listBox1.Items == null)
            {
                buttonDelete.Enabled = false;
                buttonModify.Enabled = false;

            }



        }

        const string connection = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename='C:\Users\1693082\Desktop\music app3fini\music app\music app\playlist.mdf';Integrated Security=True";

        public List<string> GetAllList()
        {
            List<string> names = new List<string>();

            string query2 = "SELECT FileName FROM MyPlay";

            SqlConnection myConnection = new SqlConnection(connection);

            try
            {
                myConnection.Open();


                SqlCommand view = new SqlCommand(query2, myConnection);
                SqlDataReader dataReader = view.ExecuteReader();

                while (dataReader.Read())
                {
                    names.Add(dataReader.GetValue(0).ToString());
                }
            }
            catch (Exception ee)
            {
                throw;
            }
            finally
            {
                myConnection.Close();
            }
            return names;
        }

        private void Refresh()
        {

            comboBox1.Items.Clear();
            foreach (string name in GetAllList())
            {
                comboBox1.Items.Add(name);
            }

        }

        private void Refresh2()
        {

            listBox1.Items.Clear();
            foreach (string name2 in GetAllList2())
            {
                listBox1.Items.Add(name2);
            }

        }
        private void Upload_Click(object sender, EventArgs e)
        {
            var file = GetFile();
            var mylist = new string[] { ".mp3", ".avi", ".mp4", ".wmv" };

            if (!mylist.Contains(Path.GetExtension(file)))
            {
                MessageBox.Show("Please select proper file type mp3,avi,mp4,wmv .");
            }
            else if (comboBox1.Items.Contains(Path.GetFileName(file)))
            {
                MessageBox.Show("This song has been added already!");
            }
            else
            {
                var result = SaveToDataBase(Path.GetFileName(file), file);
                if (result)
                {
                    comboBox1.Items.Add(Path.GetFileName(file));
                    MessageBox.Show("File successfuly added to playlist!");
                    Refresh();
                    Refresh2();
                }
            }
        }

        private void autoPlay()
        {
            axWindowsMediaPlayer1.URL = GetAddress();//GetFromDataBase(comboBox1.SelectedItem.ToString());
            //axWindowsMediaPlayer1.settings.autoStart = true;
        }

        public string GetAddress()
        {
            if(comboBox1.SelectedItem != null)
            {
                string query2 = "SELECT Location FROM MyPlay WHERE FileName = '" + comboBox1.SelectedItem.ToString() + "'";
                string answer = callSameCode(query2);
                return answer;
            }
            else
            {
                return null;
            }
        }
        

        public string GetImageAddress()
        {
            if(comboBox1.SelectedItem == null)
            {
                return null;
            }
            else
            {
                string query2 = "SELECT Location FROM AlbumImage WHERE FileName = '" + comboBox1.SelectedItem.ToString() + "'";
                string answer = callSameCode(query2);
                return answer;
            }
        }
        public string callSameCode(string query2)
        {
            string address;
            SqlConnection myConnection = new SqlConnection(connection);

            try
            {
                myConnection.Open();


                SqlCommand view = new SqlCommand(query2, myConnection);
                SqlDataReader dataReader = view.ExecuteReader();

                //while (dataReader.Read())   //Advance to next row
                if (dataReader.Read())
                {
                    address = dataReader.GetString(0);
                    return address;
                }
                return null;
                ;

            }
            catch (Exception ee)     //Caught an error
            {
                throw;
            }
            finally     //Well, either way(error or not) do this
            {
                myConnection.Close();    //Close connection so others can use the database
            }
        }

        private string GetFile()
        {
            try
            {
                openFileDialog1.Filter = "Solution Files (*.*)|*.*";
                openFileDialog1.Multiselect = false;
                if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    return openFileDialog1.FileName;
                }
            }
            catch (Exception)
            {
                throw;
            }
            return string.Empty;
        }
        private string GetImage()
        {
            try
            {
                openFileDialog2.Filter = "All files(*.*)|*.*";
                openFileDialog2.Multiselect = false;//select multiple objects
                DialogResult dr = openFileDialog2.ShowDialog();
                if (dr == System.Windows.Forms.DialogResult.OK)
                {
                    return openFileDialog2.FileName;

                }
            }
            catch (Exception)
            {
                throw;
            }
            return string.Empty;
        }

        private bool SaveToDataBase(string fileName, string location)
        {
            try
            {
                var ds = new DataSet();
                SqlCommand cmd = new SqlCommand("INSERT INTO MyPlay values('" + fileName + "','" + "NULL" + "','" + "NULL" + "','" + "NULL" + "' ,'" + 'N' + "','" + location + "'  )");


                DUSTUFF(cmd);
                return true;
            }
            catch (Exception)
            {
                throw;
            }
            return false;
        }
        private bool SaveImage(string filename, string location/*byte[]data*/)
        {
            try
            {
           
                SqlCommand cmd = new SqlCommand("INSERT INTO AlbumImage values(@name, @loca)");
                cmd.Parameters.AddWithValue("@name", filename);
                cmd.Parameters.AddWithValue("@loca", location);
            
                DUSTUFF(cmd);
                return true;
            }
            catch (Exception)
            {
                throw;
            }
            return false;
        }
        
        private void buttonDelete_Click(object sender, EventArgs e)
        {
            
            if (comboBox1.SelectedItem == null)
            {
               // MessageBox.Show("Please select file to delete.");
               
            }
            else
            {
                string delete_name = comboBox1.SelectedItem.ToString();
                String Query_delete = "DELETE FROM MyPlay WHERE FileName = @deleteName";
                String Query_deleteImage = "DELETE FROM AlbumImage WHERE FileName = @deleteName";
                SqlConnection myConnection = new SqlConnection(connection);

                try
                {
                    myConnection.Open();
                    SqlCommand cmd = new SqlCommand(Query_delete, myConnection);
                    if(pictureBox1.Image != null)
                    {
                        SqlCommand cmd2 = new SqlCommand(Query_deleteImage, myConnection);
                        cmd2.Parameters.AddWithValue("@deleteName", delete_name);
                        DUSTUFF(cmd2);
                    }
                    cmd.Parameters.AddWithValue("@deleteName", delete_name);
                    DUSTUFF(cmd);
                    //pictureBox1.Image = null;
                    MessageBox.Show("Delete Successfully!");

                    comboBox1.SelectedItem = null;
                    axWindowsMediaPlayer1.URL = null;
                    textBoxArtist.Text = null;
                    textBoxAlbum.Text = null;
                    textBoxCate.Text = null;
                    textBoxFileName.Text = null;
                    Refresh();
                    Refresh2();
                    axWindowsMediaPlayer1.close();

                }
                catch (System.Data.SqlClient.SqlException ex)
                {
                    throw;
                }
                finally
                {
                    myConnection.Close();
                }
            }

        }
        public void codeForLoadingImage()
        {
            if (GetImageAddress() != null)
            {
                pictureBox1.Load(GetImageAddress());
            }
            else
            {
                pictureBox1.Image = null;
            }
        }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            listBox1.SelectedItem = comboBox1.SelectedItem;
            codeForLoadingImage();
            
            autoPlay();

            SqlConnection myConnection = new SqlConnection(connection);
            if (comboBox1.SelectedItem != null)
            {
                buttonDelete.Enabled = true;
                buttonModify.Enabled = true;
                try
                {
                    myConnection.Open();
                    string query = "SELECT * FROM MyPlay WHERE FileName = '" + comboBox1.SelectedItem.ToString() + "'";
                    SqlCommand select = new SqlCommand(query, myConnection);

                    SqlDataReader myReader;

                    myReader = select.ExecuteReader();

                    while (myReader.Read())
                    {
                        string artist = myReader.GetString(1);
                        string album = myReader.GetString(2);
                        string category = myReader.GetString(3);
                        string filename = myReader.GetString(0);

                        textBoxArtist.Text = artist;
                        textBoxAlbum.Text = album;
                        textBoxCate.Text = category;
                        textBoxFileName.Text = filename;

                    }
                }
                catch
                {
                    throw;
                }

                finally
                {
                    myConnection.Close();
                }
            }
            else {
                buttonDelete.Enabled = false;
                buttonModify.Enabled = false;
            }
            
        }



        private void buttonModify_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem == null)
            {
                MessageBox.Show("Please select file to modify.");
            }
            else
            {
                SqlConnection myConnection = new SqlConnection(connection);

                string artist = textBoxArtist.Text;
                string albumName = textBoxAlbum.Text;
                string category = textBoxCate.Text;

               
                string query1 = "UPDATE MyPlay SET Artist = @artist, Album = @albumName, Category = @category WHERE FileName ='" + comboBox1.SelectedItem.ToString() + "'";
              

                try
                {

                    myConnection.Open();
                    SqlCommand cmd1 = new SqlCommand(query1, myConnection);
                    cmd1.Parameters.AddWithValue("@artist", artist);
                    cmd1.Parameters.AddWithValue("@albumName", albumName);
                    cmd1.Parameters.AddWithValue("@category", category);
                    
                    cmd1.CommandType = CommandType.Text;
                    cmd1.ExecuteNonQuery();
                    MessageBox.Show("Modify Successfully!");
                               
                }
                catch (System.Data.SqlClient.SqlException ex)
                {
                    throw;
                }
                finally
                {
                    myConnection.Close();
                }

            }
        }

        private void buttonAddToList_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem == null)
            {
                MessageBox.Show("Please select file to Add To List.");
            }
            else if (listBox1.Items.Contains(comboBox1.SelectedItem.ToString()))
            {
                MessageBox.Show("This File has been added to list already!");
            }
            else
            {
                SqlConnection myConnection = new SqlConnection(connection);
             
                string query = "UPDATE MyPlay SET AddToList = 'Y' WHERE FileName ='" + comboBox1.SelectedItem.ToString() + "'";
                try
                {
                    myConnection.Open();
                    SqlCommand cmd = new SqlCommand(query, myConnection);
                    DUSTUFF(cmd);
                    Refresh2();
                }
                catch (System.Data.SqlClient.SqlException ex)
                {
                    throw;
                }
                finally
                {
                    myConnection.Close();
                }

            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem == null)
            {
                buttonDelete.Enabled = false;
                buttonModify.Enabled = false;
               
            }

            else
            {
                buttonDelete.Enabled = true;
                buttonModify.Enabled = true;

                comboBox1.SelectedItem = listBox1.SelectedItem;
                if (GetImageAddress() != null)
                {
                    pictureBox1.Load(GetImageAddress());
                }
                else
                {
                    pictureBox1.Image = null;
                }
                autoPlay();
                SqlConnection myConnection = new SqlConnection(connection);

                try
                {
                    myConnection.Open();
                    string query = "SELECT * FROM MyPlay WHERE FileName = '" + listBox1.SelectedItem.ToString() + "'";
                    SqlCommand select = new SqlCommand(query, myConnection);
               
                    SqlDataReader myReader;

                    myReader = select.ExecuteReader();

                    while (myReader.Read())
                    {
                        string artist = myReader.GetString(1);
                        string album = myReader.GetString(2);
                        string category = myReader.GetString(3);
                        string filename = myReader.GetString(0);

                        textBoxArtist.Text = artist;
                        textBoxAlbum.Text = album;
                        textBoxCate.Text = category;
                        textBoxFileName.Text = filename;
                      
                    }
                }
                catch
                {
                    throw;
                }

                finally
                {
                    myConnection.Close();
                }

            }
        }

        public List<string> GetAllList2()
        {
            List<string> names2 = new List<string>();

            string query = "SELECT FileName FROM MyPlay WHERE AddToList = 'Y'";

            SqlConnection myConnection = new SqlConnection(connection);

            try
            {
                myConnection.Open();


                SqlCommand view = new SqlCommand(query, myConnection);
                SqlDataReader dataReader = view.ExecuteReader();

                while (dataReader.Read()) 
                {
                    names2.Add(dataReader.GetValue(0).ToString());
                }
            }
            catch (Exception ee)     
            {
                throw;
            }
            finally     
            {
                myConnection.Close();   
            }
            return names2;
        }

        private void buttonRemove_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem == null)
            {
                MessageBox.Show("Please select file ");
            }
            else
            {
                SqlConnection myConnection = new SqlConnection(connection);

                string query = "UPDATE MyPlay SET AddToList = 'N' WHERE FileName ='" + listBox1.SelectedItem.ToString() + "'";
                try
                {

                    myConnection.Open();
                    SqlCommand cmd = new SqlCommand(query, myConnection);
                    DUSTUFF(cmd);
                   
                    Refresh();
                    Refresh2();
                    axWindowsMediaPlayer1.close();

                }
                catch (System.Data.SqlClient.SqlException ex)
                {
                    throw;
                }
                finally
                {
                    myConnection.Close();
                }

            }
        }
        private void buttonSearch_Click(object sender, EventArgs e)
        {
            if (textBoxSearch.Text == null)
            {
                MessageBox.Show("Please enter your info for Research!");
            }

            listBox1.Items.Clear();
            string search_info = textBoxSearch.Text;

            SqlConnection myConnection = new SqlConnection(connection);


            List<string> names3 = new List<string>();

            string query = "SELECT FileName FROM MyPlay WHERE (FileName LIKE '%' + @search_info + '%') OR (Artist LIKE '%' + @search_info + '%') OR  (Category LIKE '%' + @search_info + '%')     ";

         
            try
            {
                myConnection.Open();
                SqlCommand view = new SqlCommand(query, myConnection);

                view.Parameters.AddWithValue("@search_info", search_info);

                SqlDataReader dataReader = view.ExecuteReader();

                while (dataReader.Read())  
                {
                    listBox1.Items.Add(dataReader.GetString(0));
                }
            
            }
            catch (Exception ee)     
            {
                throw;
            }
            finally   
            {
                myConnection.Close();   
            }
           
        }



        private void buttonFinish_Click(object sender, EventArgs e)
        {
            Refresh();
            Refresh2();
        }

        private void imageUpload_Click(object sender, EventArgs e)
        {
            if(pictureBox1.Image != null)
            {
                MessageBox.Show("Album image existed.");
            }
            else if (comboBox1.SelectedItem == null)
            {
                MessageBox.Show("Missing target song file");
            }
            else
            {
                var file = GetImage();
                var mylist = new string[] { ".png", ".jpg" };

                if (!mylist.Contains(Path.GetExtension(file)))
                {
                    MessageBox.Show("Please select proper file type.");
                }
                else
                {
                        var result = SaveImage(comboBox1.SelectedItem.ToString(), file);
                        if (result)
                        {
                            try
                            {
                                pictureBox1.Load(file);
                            }
                            catch (Exception)
                            {
                                throw;
                            }
                            MessageBox.Show("album image successfully added!");
                        }
                    

                }
            }
            
        }
        private void imageModify_Click(object sender, EventArgs e)
        {
            if(pictureBox1.Image == null)
            {
                MessageBox.Show("No target image file existed");
            }
            else
            {
                var file = GetImage();
                var mylist = new string[] { ".png", ".jpg" };

                if (!mylist.Contains(Path.GetExtension(file)))
                {
                    MessageBox.Show("Please select proper file type.");
                }
                try
                {
                 
                    SqlCommand cmd = new SqlCommand("UPDATE AlbumImage SET Location =  @loca WHERE FileName = '" + comboBox1.SelectedItem.ToString() + "'");             
                    cmd.Parameters.AddWithValue("@loca", file);
                    DUSTUFF(cmd);
                    pictureBox1.Load(GetImageAddress());
                }
                catch (Exception)
                {
                }
                
            }
        }
        public void DUSTUFF(SqlCommand cmd) {

            cmd.Connection = new SqlConnection(connection);
            cmd.CommandTimeout = 0;
            cmd.Connection.Open();
            cmd.ExecuteNonQuery();
        }
        private void axWindowsMediaPlayer1_Enter(object sender, EventArgs e)
        { 
        }
        private void axWindowsMediaPlayer1_PlayStateChange(object sender, AxWMPLib._WMPOCXEvents_PlayStateChangeEvent e)
        {
            if (axWindowsMediaPlayer1.playState == WMPLib.WMPPlayState.wmppsMediaEnded)
            {
                timer1.Interval = 100;
                timer1.Start();
                timer1.Enabled = true;
            }
        }

        private void timer1_tick(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex < comboBox1.Items.Count - 1)
            {
                comboBox1.SelectedIndex++;
                timer1.Enabled = false;
            }
            else
            {
                comboBox1.SelectedIndex = 0;
                timer1.Enabled = false;
            }
        }
    }
}
