#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
#endregion

namespace pelota_rebotante_rectangulo
{
	public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
		Rectangle rectanguloPelota;
		Rectangle rectanguloPlayer;
		Texture2D pelota;
		Texture2D player;        
		SpriteFont fuente;
		int direccionX = 1;
		int direccionY = 1;
		int anchoPantalla;
		int altoPantalla;
		int velocidad = 2;
		int velocidadPlayer = 2;
		int logro=0; 
		string mensaje = "";
		string mensaje1= "";
		string mensaje2 = "";
		string mensaje3 = "";
		Color color = Color.DarkTurquoise;
		Song musicadefondo;
		float tiempoColor = 0f;
		float tiempoLimite = 0.5f;
		bool colisiono = false;
		Animation playerAnimation;
		Vector2 spritePos;
		int c;
		string mensajeAyuda;
		bool presionandoAyuda = false;
		SoundEffect explosion;
		bool reproducirSonido = false;


        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "../../Content";	            
			graphics.IsFullScreen = true;
			anchoPantalla = GraphicsDevice.Viewport.Width - 64; //64: descuento el tamaño de la bola para que no se pase la pantalla
 			altoPantalla = GraphicsDevice.Viewport.Height - 64; //64: descuento el tamaño de la bola para que no se pase la pantalla 
			Window.Title = "Pelota rebotante";		
        }

		// cargo los valores del comienzo del juego, se tocan una vez
        protected override void Initialize()
        {
			// x: ubicacion del rectangulo en x. y: ubicacion del rectangulo en y. wight: ancho del rectangulo. height: alto del rectangulo
        	rectanguloPelota = new Rectangle (x: 200, y: 200, width: 64, height: 64);
			//x: ubicacion del rectangulo en x. y: ubicacion del rectangulo en y. wight: ancho del rectangulo. height: alto del rectangulo
			rectanguloPlayer = new Rectangle (x: 550, y: 700, width: 150, height: 64); 
			mensajeAyuda = "presione flecha derecha para ir hacia la derecha\n" +
				"presione flecha izquierda para ir hacia la izquierda\n" +
				"presione esc para salir";

		
			base.Initialize();
				
        }

		// cargo el contenido de los archivos multimedia (imagenes -sprite y fondo-, sonido,texto, efectos)
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
			pelota = Content.Load<Texture2D> ("Bola.png");
			player = Content.Load<Texture2D> ("rectangulo.png");
			fuente = Content.Load<SpriteFont> ("fuente1");
			musicadefondo = Content.Load<Song> ("fondo.wav");
			MediaPlayer.Play (musicadefondo);
			MediaPlayer.IsRepeating = true;
			MediaPlayer.Volume = 0.3f;
			playerAnimation = new Animation();
			Texture2D playerTexture = Content.Load<Texture2D>("otraexplosion");
			explosion = Content.Load<SoundEffect>("explosion.wav");

			/*spritePos = new Vector2(
				GraphicsDevice.Viewport.TitleSafeArea.X +
				GraphicsDevice.Viewport.TitleSafeArea.Width / 2,
				GraphicsDevice.Viewport.TitleSafeArea.Y +
				GraphicsDevice.Viewport.TitleSafeArea.Height / 2);  */

			//asigno las caracteristicas de la animacion ( la variable player texture, en la posicion de la pelota, ancho de 50, altura de 100, las 18 imagenes de la tira, velocidad 20 (rapido), color de fondo: rojo, tamaño de la animacion: grande. y false para que no se repita la animacion)
			playerAnimation.Initialize(playerTexture, spritePos, 50, 100, 18, 20, Color.Red, 4.0f, false);
			// asigno la posicion de la pelota a la animacion
			spritePos = new Vector2 (rectanguloPelota.X,rectanguloPelota.Y);

		}

		// Actualiza los datos del juego, tiene un ciclo interno 
        protected override void Update(GameTime gameTime)
		{
			if (Keyboard.GetState ().IsKeyDown (Keys.Escape)) { // Si presiono ESC, salgo
				Exit ();
			}
			spritePos.X = rectanguloPelota.X; //asigno la posicion de X de la pelota a la posicion X del sprite (animacion)
			spritePos.Y = rectanguloPelota.Y; //asigno la posicion de Y de la pelota a la posicion Y del sprite (animacion)
			if (rectanguloPelota.X >= anchoPantalla || rectanguloPelota.X <= 0) //si llega a los limites de X
				direccionX *= -1; // Cambio la direccion (rebota) y no se va de la pantalla
			if (rectanguloPelota.Y >= altoPantalla || rectanguloPelota.Y <= 0) //si llega a los limites de Y
				direccionY *= -1; // Cambio la direccion (rebota) y no se va de la pantalla
			if (rectanguloPelota.Y >= altoPantalla) { //si la pelota toca el limite inferior (altoPantalla)
				mensaje = "partida perdida"; //asigno un mensaje
				color = Color.Red; // cambia la pantalla a color rojo
				MediaPlayer.Volume = 0f; //apago el sonido
				playerAnimation.Position = spritePos;// - new Vector2 (0, 300); muestro la animacion
				playerAnimation.Update (gameTime);
				if (reproducirSonido == false) { 
					reproducirSonido = true; 
					explosion.Play (); //reproduzco animacion
				}
				return; //se termina.
			}
				rectanguloPelota.X += velocidad * direccionX; //comienza el movimiento desde la posicion de origen de
				rectanguloPelota.Y += velocidad * direccionY; // hacia abajo en diagonal

			//mover rectangulo de izquierda a derecha con el teclado
			if (Keyboard.GetState ().IsKeyDown (Keys.Left)) { //si presiono la tecla hacia la izquierda
				var x = rectanguloPlayer.X - velocidadPlayer; //se produce el movimiento
				if (x >= 0) //para que no se vaya de la pantalla hacia la izquierda
					rectanguloPlayer.X = x; //asigno los valores de x al player
			}
			if (Keyboard.GetState ().IsKeyDown (Keys.Right)) { //si presiono la tecla hacia la derecha
				var x = rectanguloPlayer.X + velocidadPlayer; //se produce el movimiento
				if (x <= anchoPantalla - 83) //para que no se vaya de la pantalla hacia la derecha
					rectanguloPlayer.X = x; //asigno los valores de x al player
			}
			//comprobar colision entre pelota y player
			if (rectanguloPlayer.Intersects (rectanguloPelota)) { // si se produce la interseccion entre player y pelota
				direccionY *= -1; 
				if (direccionX == 1) { 
					direccionX = 1;
				}
				else {
					direccionX = -1; 
				}

				velocidad++; //aumento la velocidad de la pelota
				velocidadPlayer++; //aumento la velocidad del jugador
				mensaje = "toco player"; //mensaje
				mensaje1 = "golpes"; //mensaje
				colisiono = true; // colisiono
				c++; //cuento 1 golpe
				mensaje2 = Convert.ToString (c); //paso el contador al mensaje
				if (c % 5 == 0) {
					logro += 1;//se llega al primer logro
					if (c % 10 == 0) {
						velocidad -= 2; //disminuye en 2 la velocidad de la pelota 
						velocidadPlayer -=2; // y el player
					} else {
						velocidad--; // disminuye en 1 la velocidad de la pelota
						velocidadPlayer--; // y el player
					}
				}
				mensaje3 = string.Format("Logro {0}", logro); //asigno el mensaje
				
			}
			//verifico el tiempo del color 
			if (colisiono) {
				if (tiempoColor >= tiempoLimite) {  //tiempoColor: tiempo que se va a mostrar el color 
					//0f			//0.5f			//tiempoLimite: tiempo que va a durar el color
					tiempoColor = 0f;			// Si se cumplio el tiempo, lo reseteo a 0 y vuelvo a contar
					color = Color.DarkTurquoise; // El color turquesa queda encendido el tiempoLimite.
					colisiono = false; // cambio la condicion para volver a verificar.
				} else {
					color = Color.OrangeRed; // si no se llega al tiempo limite, el color es naranja y
					tiempoColor += (float)gameTime.ElapsedGameTime.TotalSeconds; // Sumo 1 a tiempoColor. 
				}
			}


			//Modifico Volumen
			if (Keyboard.GetState ().IsKeyDown (Keys.F)) MediaPlayer.Volume = 0.0f;
			if (Keyboard.GetState ().IsKeyDown (Keys.F1)) MediaPlayer.Volume = 0.1f;
			if (Keyboard.GetState ().IsKeyDown (Keys.F2)) MediaPlayer.Volume = 0.2f;
			if (Keyboard.GetState ().IsKeyDown (Keys.F3)) MediaPlayer.Volume = 0.3f;
			if (Keyboard.GetState ().IsKeyDown (Keys.F4)) MediaPlayer.Volume = 0.4f;
			if (Keyboard.GetState ().IsKeyDown (Keys.F5)) MediaPlayer.Volume = 0.5f;
			if (Keyboard.GetState ().IsKeyDown (Keys.F6)) MediaPlayer.Volume = 0.6f;
			if (Keyboard.GetState ().IsKeyDown (Keys.F7)) MediaPlayer.Volume = 0.7f;
			if (Keyboard.GetState ().IsKeyDown (Keys.F8)) MediaPlayer.Volume = 0.8f;
			if (Keyboard.GetState ().IsKeyDown (Keys.F9)) MediaPlayer.Volume = 0.9f;
			if (Keyboard.GetState ().IsKeyDown (Keys.F10)) MediaPlayer.Volume = 1.0f;

			if (Keyboard.GetState ().IsKeyDown (Keys.Space)) { 
				presionandoAyuda = true; // Mientras mantenga presionado el Space, muestro la ayuda
			}
			 if (Keyboard.GetState (). IsKeyUp (Keys.Space)) 
			{
				presionandoAyuda = false; // Suelto el Space y se va la ayuda
			}

            base.Update(gameTime);
        }

		// Muestro el juego
        protected override void Draw(GameTime gameTime)
        {
			GraphicsDevice.Clear(color);

		
 			spriteBatch.Begin ();
			spriteBatch.Draw (pelota, rectanguloPelota, Color.White);
			spriteBatch.Draw (player, rectanguloPlayer, Color.White); 
			spriteBatch.DrawString (fuente, mensaje, new Vector2 (100, 100), Color.Black); //partida perdida - toco player
			spriteBatch.DrawString (fuente, mensaje1, new Vector2 (50, 50), Color.Black); //golpes
			spriteBatch.DrawString (fuente, mensaje2, new Vector2 (120, 50), Color.Black); // contador de golpes
			spriteBatch.DrawString (fuente, mensaje3, new Vector2 (200, 200), Color.Black); //logros
			playerAnimation.Draw(spriteBatch);
			spriteBatch.DrawString (fuente, "Ayuda: presione Space", new Vector2 (300, 100), Color.Black); //muestro ayuda
			if (presionandoAyuda) {
				spriteBatch.DrawString (fuente, mensajeAyuda, new Vector2 (250, 250), Color.Black);
			}
			spriteBatch.End ();
            base.Draw(gameTime);
        }
    }
}
