using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace HipparcosCatalog
{
    public class Fps
    {
        [DllImport("winmm.dll", EntryPoint = "timeGetTime")]
        public static extern uint MM_GetTime();

        public Fps()
        {
        }

        public float GetFps()
        {
            return fps;
        }

        /// <summary>
        /// обновляет счетчик - вызывается один раз за кадр
        /// </summary>
        public void Update()
        {
            //keep track of time lapse and frame count
            //  получить текущее время в секундах
            time = MM_GetTime() * 0.001f;
            //  увеличить количество кадров
            ++frames;

            //  Вычислить прошедшее время с начала отсчёта
            float elapsedTime = time - lastTime;
            //  Если прошла 1 секунда
            if (elapsedTime > 1.0f)
            {
                //  обновить число кадров в секунду
                fps = frames / elapsedTime;
                //  установить начальный отсчет
                lastTime = time;
                //  сбросить кадры в эту секунду
                frames = 0L;
            }
        }

        /// <summary>
        /// Текущее значение FPS
        /// </summary>
        float fps = 0;

        float lastTime = 0;

        /// <summary>
        /// Количество кадров за текущую секунду
        /// </summary>
        long frames = 0;

        float time = 0;

    }

}
