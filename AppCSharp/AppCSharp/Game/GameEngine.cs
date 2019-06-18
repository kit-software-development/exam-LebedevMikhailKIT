using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AppCSharp
{
    public class GameEngine
    {
        private GameForm gameForm;//объект игровой формы

        private Button[,] buttons = new Button[3, 3];//массив кнопок игрового поля
        public int player=1;//флаг, который показывает, какой игрок сейчас ходит, либо 1, либо 2
        public int room_player=2;//флаг, указывающий каким ходит игрок, по-умолчанию 2, но сервер может установить 1
        public bool game_status = false;//флаг, показывающий запущена\остановлена игра
        private int opened_btn_cnt = 0;//счетчик кол-ва открытых кнопок

        //конструктор инциализации игровых механик
        public GameEngine(GameForm obj)
        {
            gameForm = obj;

            player = 1;//это флаг, который показывает, какой игрок сейчас ходит
            setButtons();//инициализация массива с кнопками, для работы с ними
            gameForm.SetNumber("2");//по-умолчанию, игрок все имеет порядковый номер для хода 2
        }

        //здесь инициализируются кнопки
        private void setButtons()
        {
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    buttons[i, j] = gameForm.Controls["GameButton" + (i + 1) + (j + 1)] as Button;
                    buttons[i, j].TabStop = false;//чтобы не было лишнего перепрыгивания
                }
            }
        }

        //проверка полученного сообщения
        public void CheckMessage(string message)
        {
            if (message == "01")//установка номера игрока в еденицу
            {
                gameForm.SetNumber("1");
                room_player = 1;
            }
            else if (message == "4")//сообщение, которое сигнализирует, что игра начата
            {
                gameForm.SetGameStatus("Ход игрока 1");
                game_status = true;
            }
            else if (message == "5")//сообщение, которое сигнализирует, что игра окончена
            {
                gameForm.connect.Disconnect();
                game_status = false;
                gameForm.SetConnectionStatus("Отсутствует");
                gameForm.SetNumber("-");
            }
            else if (message == "6")//сообщение, которое сигнализирует, что один из игроков покинул комнату, игра прерывается
            {
                MessageBox.Show("Соперник отключился!");
                gameForm.connect.Disconnect();
                game_status = false;
                gameForm.SetConnectionStatus("Отсутствует");
                gameForm.SetGameStatus("-");
                gameForm.SetNumber("-");
            }
            else if (message == "11" || message == "12" || message == "13" ||//сообщение о том, какая кнопка открыта
                     message == "21" || message == "22" || message == "23" ||//первая цифра - строка, вторая столбец
                     message == "31" || message == "32" || message == "33") 
            {
                int i = int.Parse(message.Substring(0, 1))-1;
                int j = int.Parse(message.Substring(1, 1))-1;
                OpenButton(buttons[i, j]);
            }
        }

        //открытие какой-либо из кнопок
        public void OpenButton(Button btn)
        {
            switch (player)
            {
                case 2:
                    btn.Text = "x";
                    player = 1;
                    gameForm.SetGameStatus("Ход игрока 1");
                    break;
                case 1:
                    btn.Text = "o";
                    player = 2;
                    gameForm.SetGameStatus("Ход игрока 2");
                    break;
            }

            opened_btn_cnt++;//увеличивает счетчик игры, для ничьи, т.е. если будет открыто 9 кнопок, но не будет победителя, то ничья
            btn.Enabled = false;
            checkWin();
        }

        //проверяем состояние игры
        //победа, поражение, ничья
        //проверяются все возможные линии и смотрится, что в них одинаковый текст и текст не пустой
        //текст может быть: пустой, x или o
        public void checkWin()
        {
            if (game_status)
            {
                string message;
                if (player != room_player)
                {
                    message = "Вы победили!";
                }
                else
                {
                    message = "Вы проиграли!";
                }
                if (buttons[0, 0].Text == buttons[0, 1].Text && buttons[0, 1].Text == buttons[0, 2].Text)
                {
                    if (buttons[0, 0].Text != "")
                    {
                        game_status = false;
                        try
                        {
                            gameForm.connect.SendMessage("5");
                        }
                        catch { }
                        gameForm.SetGameStatus(message);
                        MessageBox.Show(message);
                        return;
                    }
                }
                if (buttons[1, 0].Text == buttons[1, 1].Text && buttons[1, 1].Text == buttons[1, 2].Text)
                {
                    if (buttons[1, 0].Text != "")
                    {
                        game_status = false;
                        try
                        {
                            gameForm.connect.SendMessage("5");
                        }
                        catch { }
                        gameForm.SetGameStatus(message);
                        MessageBox.Show(message);
                    }
                }
                if (buttons[2, 0].Text == buttons[2, 1].Text && buttons[2, 1].Text == buttons[2, 2].Text)
                {
                    if (buttons[2, 0].Text != "")
                    {
                        game_status = false;
                        try
                        {
                            gameForm.connect.SendMessage("5");
                        }
                        catch { }
                        gameForm.SetGameStatus(message);
                        MessageBox.Show(message);
                    }
                }
                if (buttons[0, 0].Text == buttons[1, 0].Text && buttons[1, 0].Text == buttons[2, 0].Text)
                {
                    if (buttons[0, 0].Text != "")
                    {
                        game_status = false;
                        try
                        {
                            gameForm.connect.SendMessage("5");
                        }
                        catch { }
                        gameForm.SetGameStatus(message);
                        MessageBox.Show(message);
                    }
                }
                if (buttons[0, 1].Text == buttons[1, 1].Text && buttons[1, 1].Text == buttons[2, 1].Text)
                {
                    if (buttons[0, 1].Text != "")
                    {
                        game_status = false;
                        try
                        {
                            gameForm.connect.SendMessage("5");
                        }
                        catch { }
                        gameForm.SetGameStatus(message);
                        MessageBox.Show(message);
                    }
                }
                if (buttons[0, 2].Text == buttons[1, 2].Text && buttons[1, 2].Text == buttons[2, 2].Text)
                {
                    if (buttons[0, 2].Text != "")
                    {
                        game_status = false;
                        try
                        {
                            gameForm.connect.SendMessage("5");
                        }
                        catch { }
                        gameForm.SetGameStatus(message);
                        MessageBox.Show(message);
                    }
                }
                if (buttons[0, 0].Text == buttons[1, 1].Text && buttons[1, 1].Text == buttons[2, 2].Text)
                {
                    if (buttons[0, 0].Text != "")
                    {
                        game_status = false;
                        try
                        {
                            gameForm.connect.SendMessage("5");
                        }
                        catch { }
                        gameForm.SetGameStatus(message);
                        MessageBox.Show(message);
                    }
                }
                if (buttons[2, 0].Text == buttons[1, 1].Text && buttons[1, 1].Text == buttons[0, 2].Text)
                {
                    if (buttons[2, 0].Text != "")
                    {
                        game_status = false;
                        try
                        {
                            gameForm.connect.SendMessage("5");
                        }
                        catch { }
                        gameForm.SetGameStatus(message);
                        MessageBox.Show(message);
                    }
                }
                if (game_status && opened_btn_cnt == 9)//если игра не закончено, но открыто 9 кнопок, то значит ничья
                {
                    message = "Ничья";
                    game_status = false;
                    try
                    {
                        gameForm.connect.SendMessage("5");
                    }
                    catch { }
                    gameForm.SetGameStatus(message);
                    MessageBox.Show(message);
                }
            }
        }
    }
}
