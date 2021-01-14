using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Minesweeper.Core;
using SFML.Graphics;
using SFML.Window;

namespace Minesweeper
{
    class Program
    {
	    public static Random rand;
	    public static RenderWindow window;
	    private static Game game;

        static void Main(string[] args)
        {
			rand = new Random();
			game = new Game();			// создание экземпляра игры

			window = new RenderWindow(new VideoMode(320, 420), "Minesweeper", Styles.Close);	// создание окна разрешением 320x420 с названием Minesweeper

            window.Closed += Window_Closed;						// подписываемся на событие закрытия окна
	        window.MouseButtonPressed += game.OnMouseClicked;  // подписываем игру на событие нажатия кнопки мыши

            Image icon = new Image("images/Minesweeper.png");	// загружае картинку иконки
			window.SetIcon(icon.Size.X, icon.Size.Y, icon.Pixels);	// устанавливаем иконку для окна

	        while (window.IsOpen)		// бесконечный цикл пока открыто окно
	        {
		        window.DispatchEvents();    // обрабатывае события

				game.Update();          // обновляем логику игры

				window.Clear();			// очищаем окно
				game.Render(window);    // рендерим игру
                window.Display();		// отображаем на дисплее
            }

        }

        private static void Window_Closed(object sender, EventArgs e)
        {
            window.Close();		// закрываем окно
        }		// обработчик закрытия окна
    }
}
