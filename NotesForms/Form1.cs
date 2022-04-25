using System.Data.SQLite;
using System.Text.RegularExpressions;

namespace NotesForms
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
			SQLiteDataReader sql_read;
			SQLiteConnection con = new SQLiteConnection("Data Source=Notes.db");
			con.Open();

			listBox1.Items.Clear();
			SQLiteCommand sql = con.CreateCommand();
			sql.CommandText = "SELECT Title FROM Notes";

			sql.ExecuteNonQuery();
			sql_read = sql.ExecuteReader();
			while(sql_read.Read())
			{
				listBox1.Items.Add(sql_read.GetString(0));
			}
			sql_read.Close();
			con.Close();
			listBox1.SetSelected(0, true);
		}

		private void button1_Click(object sender, EventArgs e)
		{
			SQLiteDataReader sql_read;
			SQLiteConnection con = new SQLiteConnection("Data Source=Notes.db");
			con.Open();
			SQLiteCommand sql = con.CreateCommand();

			string title = label1.Text;
			string str = textBox1.Text;
			sql.CommandText = "SELECT Title, Text FROM Notes";
			sql.ExecuteNonQuery();
			sql_read = sql.ExecuteReader();
			while(sql_read.Read())
			{
				if(title.Equals(sql_read.GetString(0)))
				{
					if(str != sql_read.GetString(1))
					{
						string message = "Изменить эту заметку?";
						string caption = "Изменение заметки";
						var result = MessageBox.Show(message, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

						if(result == DialogResult.Yes)
						{
							Regex rgx = new Regex(@"'");
							if(!rgx.IsMatch(str))
							{
								SQLiteCommand sql1 = con.CreateCommand();
								sql1.CommandText = $"UPDATE Notes set Text = '{str}' where Title = '{title}'";
								sql1.ExecuteNonQuery();
							}
							else
							{
								message = "Символ «'» является недопустимым, замените его другим сиволом или уберите его из текста.";
								caption = "Встречен недопустимый символ";
								result = MessageBox.Show(message, caption, MessageBoxButtons.OK, MessageBoxIcon.Information);
							}
						}
					}
				}
			}
			sql_read.Close();
			con.Close();
		}

		private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
		{
			if(listBox1.SelectedItem != null)
			{
				SQLiteDataReader sql_read;
				string curItem = listBox1.SelectedItem.ToString();
				SQLiteConnection con = new SQLiteConnection("Data Source=Notes.db");
				con.Open();

				SQLiteCommand sql = con.CreateCommand();
				sql.CommandText = "SELECT Title, Text FROM Notes";

				sql.ExecuteNonQuery();
				sql_read = sql.ExecuteReader();
				while(sql_read.Read())
				{
					if(curItem.Equals(sql_read.GetString(0)))
					{
						textBox1.Text = sql_read.GetString(1);
						label1.Text = sql_read.GetString(0);
						button1.Visible = true;
						button2.Visible = true;
					}

				}
				sql_read.Close();
				con.Close();
			}
		}

		private void button2_Click(object sender, EventArgs e)
		{
			const string message = "Вы хотите удалить эту заметку?";
			const string caption = "Удаление заметки";
			var result = MessageBox.Show(message, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

			if(result == DialogResult.Yes)
			{
				SQLiteDataReader sql_read;
				SQLiteConnection con = new SQLiteConnection("Data Source=Notes.db");
				con.Open();
				SQLiteCommand sql = con.CreateCommand();

				string title = label1.Text;
				sql.CommandText = "SELECT Title FROM Notes";
				sql.ExecuteNonQuery();
				sql_read = sql.ExecuteReader();
				while(sql_read.Read())
				{
					if(title.Equals(sql_read.GetString(0)))
					{
						SQLiteCommand sql1 = con.CreateCommand();
						sql1.CommandText = $"DELETE from Notes where Title = '{title}'";
						sql1.ExecuteNonQuery();
						listBox1.Items.Remove(title);
						textBox1.Text = "";
						label1.Text = "Название заметки";
						button1.Visible = false;
						button2.Visible = false;
					}
				}
				sql_read.Close();
				con.Close();
			}
		}

		private void button3_Click(object sender, EventArgs e)
		{
			SQLiteDataReader sql_read;
			SQLiteConnection con = new SQLiteConnection("Data Source=Notes.db");
			con.Open();
			SQLiteCommand sql = con.CreateCommand();
			string title = textBox2.Text;
			string text = textBox3.Text;
			if(title != "")
			{
				int err = 0;
				sql.CommandText = "SELECT Title FROM Notes";
				sql.ExecuteNonQuery();
				sql_read = sql.ExecuteReader();
				while(sql_read.Read())
				{
					if(title == sql_read.GetString(0))
					{
						err = 1;
					}
				}
				sql_read.Close();
				if(err != 1)
				{
					Regex rgx = new Regex(@"'");
					if(!rgx.IsMatch(text) && !rgx.IsMatch(title))
					{
						sql.CommandText = $"INSERT INTO Notes values(NULL, '{title}', '{text}')";
						sql.ExecuteNonQuery();
						con.Close();
						listBox1.Items.Add(title);
						textBox2.Text = "";
						textBox3.Text = "";
					}
					else
					{
						const string message = "Символ «'» является недопустимым, замените его другим сиволом или уберите его.";
						const string caption = "Встречен недопустимый символ";
						var result = MessageBox.Show(message, caption, MessageBoxButtons.OK, MessageBoxIcon.Information);
					}
				}
				else
				{
					const string message = "Такая заметка уже существует, измените её название.";
					const string caption = "Измените название заметки";
					var result = MessageBox.Show(message, caption, MessageBoxButtons.OK, MessageBoxIcon.Information);
				}
			}
			else
			{
				const string message = "Заполните поле «Название заметки»!";
				const string caption = "Ошибка";
				var result = MessageBox.Show(message, caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
	}
}