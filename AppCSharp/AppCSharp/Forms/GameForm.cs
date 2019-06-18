using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.Security;
namespace AppCSharp
{
    public partial class GameForm : Form
    {
        public GameEngine game;//объект игры
        public Connection connect;//объект соединения

        public GameForm()
        {
            InitializeComponent();
        }

        //при первой загрузке формы инициализируем основные объекты
        private void GameForm_Load(object sender, EventArgs e)
        {
            game = new GameEngine(this);
            connect = new Connection(this);
        }

        //нажатие на кнопку игрового поля
        private void GameButton_Click(object sender, EventArgs e)
        {
            //если игра запущена и ход игрока, то нажимает
            if (game.player == game.room_player && game.game_status)
            {
                Button btn = sender as Button;//получаем обхект кнопки
                connect.SendMessage(btn.Name.Substring(btn.Name.Length - 2, 2));//отправляем какая кнопка была нажата
                game.OpenButton(btn);//вызывает процедуру открытия кнопки
                game.checkWin();//проверяем была ли закончена игра
            }
        }

        //процедура смены текста состояния соединения
        public void SetConnectionStatus(string msg)
        {
            ConnectionStaus.Text = msg;
        }

        //процедура смены текста состояния игры
        public void SetGameStatus(string msg)
        {
            GameStatus.Text = msg;
        }

        //процедура смены номера игрока
        public void SetNumber(string msg)
        {
            Number.Text = msg;
        }

        //процедура рестарта игры
        private void NewGame_Click(object sender, EventArgs e)
        {
            Application.Restart();
        }
    }
}
