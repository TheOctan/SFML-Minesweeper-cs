using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;
using SFML.System;

namespace Minesweeper.Util
{
    public class Timer : Transformable, Drawable    // наследуется от Drawable - отвечает за отрисовку, Trnsformable - отвечает за трансформацию
    {
        private Clock clock;            // часы
        private Text textTime;          // текст
        private RectangleShape box;     // рамка
        private uint time;            // время

        private bool isUpdate;          // статус обновления

        public Timer(Font font)
        {
            clock = new Clock();
            textTime = new Text("0", font, 50)
            {
                Color = Color.Red
            };
            box = new RectangleShape(new Vector2f(100, 60))
            {
                FillColor = Color.Black
            };

            time = 0;
            isUpdate = false;
        }

        public void Start()
        {
            if (!isUpdate)          // если часы не обновляются
                clock.Restart();    // сбрасываем таймер
            isUpdate = true;        // обновляем часы
        }

        public void Stop()
        {
            isUpdate = false;       // прекращаем обновление
        }

        public void Restart()
        {
            time = 0;                                       // сбрасываем время
            textTime.DisplayedString = time.ToString();     // устанавливаем текст равный текущему времени
        }

        public void Update()
        {
            if (isUpdate)       // если обновляется
            {
                time = (uint)clock.ElapsedTime.AsSeconds();    // получаем у часов текущее время в секундах
                if (time > 999)                                 // если время больше 999 секунд
                    time = 0;                                   // сбрасываем его

                textTime.DisplayedString = time.ToString();     // устанавливаем текст равный текущему времени
            }
        }

        public void Draw(RenderTarget target, RenderStates states)
        {
            states.Transform *= Transform;		// перемножаем внешнюю трансформацию на текущую

            target.Draw(box, states);			// отрисовываем бокс в указанную цель с состоянием трансформации
			target.Draw(textTime, states);		// также текст
        }
    }
}
