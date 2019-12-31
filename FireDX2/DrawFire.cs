using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FireDX2
{
    class DrawFire
    {
        Texture2D tFire = null;
        private Color[] fireColors = null;

        private int widthFire = 0;
        private int heightFire = 0;
        private Random randomFire = new Random();

        private static int fireBottom = 2;
        private static int fireMargin = 5;
        private static int coolThreshhold = 2;

        // Contructor for fire, taking a Texture2D created outside of class
        public DrawFire(Texture2D textureFire)
        {
            tFire = textureFire;
            fireColors = new Color[tFire.Width * tFire.Height];
            widthFire = tFire.Width;
            heightFire = tFire.Height;
        }

        // Create random pixels to kick off the fire, offset from bottom
        public void randomPixels(int rowOffset)
        {
            int startFire = (heightFire - rowOffset) * widthFire;

            for (int k = startFire; k < startFire + widthFire; k++)
            {
                fireColors[k].R = (byte)randomFire.Next(100, 255);
                fireColors[k].G = (byte)randomFire.Next(0, 70);
                fireColors[k].B = (byte)randomFire.Next(0, 40);
            }

            // always add a hot middle
            //for (int j = startFire + (widthFire / 3); j < startFire + ((widthFire / 3) * 2); j++)
            //    if (randomFire.Next(0, 4) == 1) fireColors[j] = Color.Yellow;

            // black sides
            for (int b = startFire; b < startFire + fireMargin; b++)
                fireColors[b] = Color.Black;

            for (int b = startFire + widthFire - fireMargin - 1; b < startFire + widthFire -1; b++)
                fireColors[b] = Color.Black;
        }

        // Insert some random pixels with a chosen colour to bias the flames
        public void insertPixels(int rowOffset, Color insertColour, int chanceofInsert)
        {
            int startFire = (heightFire - rowOffset) * widthFire;

            for (int k = startFire; k < startFire + widthFire; k++)
            {
                if (randomFire.Next(0, 100) <= chanceofInsert)
                {
                    fireColors[k].R = insertColour.R;
                    fireColors[k].G = insertColour.G;
                    fireColors[k].B = insertColour.B;
                }
            }
        }

        // Update color array to achieve a basic flame effect
        public void updateFire()
        {
            int startFireRow = 0, nextFireRow = 0;

            // just abort if we have no fire started
            if (tFire == null) return;

            // Step 1 - Randomly seed bottom of flames
            for (int j = 1; j < fireBottom; j++) randomPixels(j);

            // Step 2 - Bias in some more Orange and Red
            for (int j = 1; j < fireBottom; j++) insertPixels(j, Color.Orange, 10);
            for (int j = 1; j < fireBottom; j++) insertPixels(j, Color.Yellow, 10);
            for (int j = 1; j < fireBottom; j++) insertPixels(j, Color.DarkRed, 10);

            // Step 3 - Move flame upwards
            for (int j = 0; j < heightFire - 1; j++)
            {
                startFireRow = j * widthFire;
                nextFireRow = (j + 1) * widthFire;

                for (int i = startFireRow+1; i < (startFireRow + widthFire - 1); i++)
                {
                    fireColors[i] = fireColors[i + widthFire];
                }
            }

            // Step 4 - Smooth and cool the flame
            int meanR = 0, meanG = 0, meanB = 0;
            Color pixelThis = Color.Black, pixelAbove = Color.Black, pixelAboveLeft = Color.Black, pixelAboveRight = Color.Black;
            for (int j = 1; j < heightFire - 1; j++)
            {
                startFireRow = j * widthFire;
                nextFireRow = (j + 1) * widthFire;
                for (int i = startFireRow+1; i < (startFireRow + widthFire - 1); i++)
                {
                    // get pixels to process
                    pixelThis = fireColors[i];
                    pixelAbove = fireColors[i + widthFire];
                    pixelAboveLeft = fireColors[i + widthFire - 1];
                    pixelAboveRight = fireColors[i + widthFire + 1];

                    // average out the RGB
                    meanR = (pixelThis.R + pixelAbove.R + pixelAboveLeft.R + pixelAboveRight.R) / 4;
                    meanG = (pixelThis.G + pixelAbove.G + pixelAboveLeft.G + pixelAboveRight.G) / 4;
                    meanB = (pixelThis.B + pixelAbove.B + pixelAboveLeft.B + pixelAboveRight.B) / 4;

                    // cool using a threshold
                    meanR = (meanR < coolThreshhold) ? 0 : meanR - randomFire.Next(coolThreshhold);
                    meanG = (meanG < coolThreshhold) ? 0 : meanG - randomFire.Next(coolThreshhold);
                    meanB = (meanB < coolThreshhold) ? 0 : meanB - randomFire.Next(coolThreshhold);

                    // New value for current pixel
                    fireColors[i].R = (byte)meanR;
                    fireColors[i].G = (byte)meanG;
                    fireColors[i].B = (byte)meanB;
                }
            }

            // Replace the Texture2D
            tFire.SetData<Color>(fireColors);

        }
    }
}
