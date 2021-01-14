using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Minesweeper.GUI;
using Minesweeper.Util;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace Minesweeper.Core
{
    public class Game
    {
        private readonly Texture texture;		// текстура игры
        private readonly Font font;				// шрифт

        private RectangleShape menu;			// рамка меню
        private RectangleShape box;				// бок счётчика

        private Text flags;						// текст отображения количесва фагов
        private Timer timer;					// таймер
        private Button button;					// кнопка
        private Field field;					// игровое поле

	    private Vector2i pos;					// позиция мышки

        public Game()
        {
            texture = new Texture("images/tiles.jpg");
            font = new Font("Fonts/digifaw.TTF");

            menu = new RectangleShape()
            {
                Size = new Vector2f(320f, 100f),
                FillColor = new Color(192, 192, 192),
                OutlineColor = new Color(128, 128, 128),
                OutlineThickness = -3
            };

            box = new RectangleShape()
            {
                Size = new Vector2f(100, 60),
                FillColor = Color.Black,
                Position = new Vector2f(20f, 20f)
            };

            flags = new Text("0", font, 50)
            {
                Color = Color.Red,
                Position = new Vector2f(20f, 20f)
            };

            timer = new Timer(font)
            {
                Position = new Vector2f(200, 20)
            };

            button = new Button(new Vector2f(60, 60))
            {
                Position = new Vector2f(130, 20)
            };
	        button.Clicked += OnButtonClicked;

            field = new Field(texture, new Vector2i(10, 10), 10)
            {
                Position = new Vector2f(0, 100)
            };
        }

        public void Update()
	    {
		    pos = Mouse.GetPosition(Program.window);       // полчаем позицию мыши относительно окна

			button.Updade(pos);								// обновляем кнопку
			timer.Update();									// обновляем таймер

		    flags.DisplayedString = field.Flags.ToString();	// устанавливаем текст равный количеству неустановленных флагов

		    if (field.IsGameOver)	// если конец игры
		    {
				timer.Stop();
				button.FillColor = Color.Red;
		    }
			else if (field.IsGameWin) // иначе если выйгрыш
		    {
				timer.Stop();
				button.FillColor = Color.Green;
		    }

	    }			// обновление логики

	    public void Render(RenderTarget target)
	    {
			// отрисовываем
			target.Draw(menu);		// меню
			target.Draw(button);	// кнопку
			target.Draw(timer);		// таймер
			target.Draw(box);		// бокс счётчика
			target.Draw(flags);		// счётчик флагов
			target.Draw(field);     // игровое поле

        }			// рендер игры

        private void OnButtonClicked(object sender, EventArgs e)
        {
            timer.Stop();		// останавливаем таймер
			timer.Restart();	// рестартим

			field.Reset();		// сбрасываем игровое поле

        }	// обработчик нажатия кнопки

	    public void OnMouseClicked(object sender, MouseButtonEventArgs e)
	    {
			field.Update(pos, e.Button);	// по нажатию мышки обновляем игровое поле
			if(!field.IsGameOver && !field.IsGameWin)	// если не конец игры и не выйгрыш
				timer.Start();							// стартуем таймер
	    }			// обработчик нажатия мыши
    }
}
