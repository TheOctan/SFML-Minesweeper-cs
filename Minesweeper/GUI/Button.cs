using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace Minesweeper.GUI
{
    public class Button : Transformable, Drawable //класс кнопка (наследуется от Drawable - отвечает за отрисовку, Trnsformable - отвечает за трансформацию)
    {
	    public event EventHandler Clicked = (object sender, EventArgs e) => { };            // событие нажатия кнопки

        public Vector2f Size { get => box.Size; set => box.Size = value; }							// размер кнопки
        public Color FillColor { get => box.FillColor; set => box.FillColor = value; }              // цвет кнопки

	    private bool clickable;                             //отвечает, кликабельна ли кнопка

	    private RectangleShape box;                         //рамка

        public Button() : this(new Vector2f(100, 30))
        {

        }
        public Button(Vector2f size)
        {
	        box = new RectangleShape()
	        {
				Size = size,
				FillColor = Color.Transparent,
				OutlineColor = Color.White,
				OutlineThickness = -1
	        };
	        clickable = true;
        }

	    public void Updade(Vector2i pos)
	    {
		    var correct = new Vector2f(pos.X, pos.Y) - Position;		// корректируем позицию мыши относительно позиции кнопки (свойство Position наследовано от Transformable)

		    if (box.GetGlobalBounds().Contains(correct.X, correct.Y))	// если бокс содержит в себе координаты мыши
		    {
				box.FillColor = new Color(239, 228, 176, 150);			// то перекрашиваем бокс

			    if (Mouse.IsButtonPressed(Mouse.Button.Left))			// если нажата правая кнопка мыши
			    {
					box.FillColor = new Color(249, 176, 166, 150);      // то перекрашиваем бокс

				    if (clickable)			// если кнопка кликабельна
				    {
						Clicked(this, EventArgs.Empty);
					    clickable = false;	// сановится не кликабельной
				    }
                }
			    else													// иначе если не нажата, то кнопка кликабельна
			    {
				    clickable = true;		
			    }
		    }
		    else
		    {
			    box.FillColor = Color.Transparent;
		    }
        }               //метод обновления

        public void Draw(RenderTarget target, RenderStates states)
        {
	        states.Transform *= Transform;		// перемножаем внешнюю трансформацию на текущую

			target.Draw(box, states);			// отрисовываем бокс в указанную цель с состоянием трансформации
        }
    }
}
