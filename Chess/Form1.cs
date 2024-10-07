using System.Collections.Immutable;
using System.Drawing.Design;

namespace Chess
{
    public partial class Form1 : Form
    {
        string[,] fichas = { { "♖", "♘", "♗", "♕", "♔", "♗", "♘", "♖" }
                           , { "♙", "♙", "♙", "♙", "♙", "♙", "♙", "♙" }, 
                             {"", "", "", "", "", "", "", ""},
                             {"", "", "", "", "", "", "", ""},
                             {"", "", "", "", "", "", "", ""},
                             {"", "", "", "", "", "", "", ""},
                             {"♟", "♟", "♟", "♟", "♟", "♟", "♟", "♟"},
                             {"♜", "♞", "♝", "♛", "♚", "♝","♞", "♜"}
                            };
        int celda = 50;
        Button[,] tablero = new Button[8, 8];
        
        int[] selected = [];
        bool turnoBlanca = true;
        
        string[] blancas = { "♖", "♘", "♗", "♕", "♙", "♔"};
        string[] negras = { "♜", "♞", "♝", "♛", "♚", "♟" };
            
        Label lbl_info = new Label();
        Label lbl_tiempoBlancas = new Label();
        
        int tiempoRestanteBlancas = 300000;
        int tiempoRestanteNegras = 300000;
        System.Windows.Forms.Timer cronometro = new System.Windows.Forms.Timer();

        public Form1()
        {
            InitializeComponent();
            this.Size = new System.Drawing.Size(celda * 8 + 20, celda * 8 + 80);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            lbl_info.Location = new Point(0, celda * 8 + 5);
            lbl_info.Size = new Size(320, 15);
            lbl_info.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            MostrarInfoFichas();
            this.Controls.Add(lbl_info);
            
            lbl_tiempoBlancas.Location = new Point(0, celda * 8 + 20);
            lbl_tiempoBlancas.Size = new Size(320, 15);
            lbl_tiempoBlancas.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            this.Controls.Add(lbl_tiempoBlancas);
            
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {

                    Button boton = new Button();
                    boton.Text = fichas[i, j];
                    boton.Size = new Size(celda, celda);
                    boton.Location = new Point(j * celda, i * celda);
                    boton.Font = new Font("Arial", 30, FontStyle.Bold);
                    tablero[i, j] = boton;

                    int px = i;
                    int py = j;
                    string ficha = fichas[i, j];
                    boton.Click += (Sender, r) => Seleccionar(px, py);

                    this.Controls.Add(tablero[i, j]);       
                }
            }
            LimpiarFondo();

            cronometro.Interval = 1000;
            cronometro.Tick += new EventHandler(ActualizarTiempo);
            cronometro.Start();
        }
                
        public void ActualizarTiempo(object sender, EventArgs e)
        {
            if (turnoBlanca) tiempoRestanteBlancas -= 1000;
            if (!turnoBlanca) tiempoRestanteNegras -= 1000;
            string stringTiempoBlancas = ConvertirMilisegundosAString(tiempoRestanteBlancas);
            string stringTiempoNegras = ConvertirMilisegundosAString(tiempoRestanteNegras);
            lbl_tiempoBlancas.Text = $"Blancas: {stringTiempoBlancas} - Negras: {stringTiempoNegras}";
            if (tiempoRestanteNegras == 0)
            {
                cronometro.Stop();
                MessageBox.Show("Se ha acabado el tiempo, Las fichas blancas han ganado");
                this.Close();
            } else if (tiempoRestanteBlancas == 0)
            {
                cronometro.Stop();
                MessageBox.Show("Se ha acabado el tiempo, Las fichas negras han ganado");
                this.Close();

            }
        }
        
        public string ConvertirMilisegundosAString(int milisegundos)
        {
            int segundosTotales = milisegundos / 1000;
            int minutos = segundosTotales / 60;
            int segundos = segundosTotales % 60;
            return $"{minutos}:{segundos:D2}";
        }
        
        public void MostrarInfoFichas()
        {
            int numBlancas = 0;
            int numNegras = 0;
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (blancas.Contains(fichas[i, j])) numBlancas++;
                    if (negras.Contains(fichas[i, j])) numNegras++;
                }
            }
            lbl_info.Text = $"Blancas: {numBlancas}; Negras: {numNegras}";
        }

        private void Seleccionar(int px, int py)
        {
            if (selected.Length == 2)
            {
                if (VerificarEsLaMismaPieza(px, py))
                {
                    selected = [];
                    LimpiarFondo();
                    return;
                }
                Movimiento(selected, [px, py]);
                selected = [];
                return;
            }
            if (!VerificarTurno(px, py)) return;
            if (tablero[px, py].Text == "") return;
            LimpiarFondo();
            selected = [px, py];
            tablero[selected[0], selected[1]].BackColor = Color.Green;
            DrawMovements(px, py);
        }
        
        private void Movimiento(int[] previous, int[] current)
        {
            if (tablero[previous[0], previous[1]].Text != "")
            {
                bool esRojo = tablero[current[0], current[1]].BackColor.Equals(Color.Red);
                if (!esRojo) return;
                fichas[current[0], current[1]] = fichas[previous[0], previous[1]];
                fichas[previous[0], previous[1]] = "";
                tablero[current[0], current[1]].Text = tablero[previous[0], previous[1]].Text;
                tablero[previous[0], previous[1]].Text = "";
                turnoBlanca = !turnoBlanca;
                LimpiarFondo();
            }
            MostrarInfoFichas();
            ValidarHayGanador();
        }

        private void LimpiarFondo()
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    tablero[i, j].BackColor = (i + j) % 2 == 0 ? Color.LightPink : Color.Beige;
                }
            }
        }

        private bool VerificarEsLaMismaPieza(int px, int py)
        {
            return selected[0] == px && selected[1] == py;
        }

        private bool VerificarTurno(int px, int py)
        {
            if (turnoBlanca)
            {
                return blancas.Contains(tablero[px, py].Text);
            }
            return negras.Contains(tablero[px, py].Text);
        }

        private void DrawMovements(int px, int py)
        {
            string piece = tablero[px, py].Text;

            switch (piece)
            {
                case "♙" or "♟":
                    MovimientosPeon(px, py, turnoBlanca);
                    break;
                case "♗" or "♝":
                    List<List<int>> sumadorAlfil = new List<List<int>>([[1, 1], [1, -1], [-1, 1], [-1, -1]]);
                    MovimientosDirigido(px, py, turnoBlanca, sumadorAlfil);
                    break;
                case "♜" or "♖":
                    List<List<int>> sumadorTorre = new List<List<int>>([[1, 0], [0, 1], [-1, 0], [0, -1]]);
                    MovimientosDirigido(px, py, turnoBlanca, sumadorTorre);
                    break;
                case "♘" or "♞":
                    MovimientosCaballo(px, py, turnoBlanca);
                    break;
                case "♚" or "♔":
                    MovimientosRey(px, py, turnoBlanca);
                    break;
                case "♛" or "♕":
                    List<List<int>> sumadorRectos = new List<List<int>>([[1, 0], [0, 1], [-1, 0], [0, -1]]);
                    MovimientosDirigido(px, py, turnoBlanca, sumadorRectos);
                    List<List<int>> sumadorDiagonales = new List<List<int>>([[1, 1], [1, -1], [-1, 1], [-1, -1]]);
                    MovimientosDirigido(px, py, turnoBlanca, sumadorDiagonales);
                    break;
            }
        }
        
        private void MovimientosPeon(int px, int py, bool esBlanco)
        {
            bool esPrimerMovimiento = (px == 1 && esBlanco) || (px == 6 && !esBlanco);
            int multiplicador = esBlanco ? 1 : -1;
            PintarPosiblesAtaquesPeon(px, py, esBlanco);
            for (int i = 1; i <= (esPrimerMovimiento ? 2 : 1); i++)
            {
                if (!(tablero[px + (i * multiplicador), py].Text == "")) break;
                tablero[px + (i * multiplicador), py].BackColor = Color.Red;
            }
        }
        
        private void PintarPosiblesAtaquesPeon(int px, int py, bool esBlanco) {
            string[] enemigos = esBlanco ? negras : blancas;
            List<int[]> posiblesAtaques = new List<int[]>([[px - 1, py - 1], [px - 1, py + 1], [px + 1, py - 1], [px + 1, py + 1]]);
            foreach (int[] posicion in posiblesAtaques)
            {
                if (ValidarIndiceFueraDeRango(posicion)) continue;
                Button fichaActual = tablero[posicion[0], posicion[1]];
                if (!enemigos.Contains(fichaActual.Text)) continue;
                fichaActual.BackColor = Color.Red;   
            }
        }
            
        private bool ValidarIndiceFueraDeRango(int[] posicion)
        {
            bool indiceFueraDeRango = !(0 <= posicion[0] && posicion[0] < 8 && 0 <= posicion[1] && posicion[1] < 8);
            return indiceFueraDeRango;

        }

        private void MovimientosDirigido(int px, int py, bool esBlanco, List<List<int>> sumadores)
        {           
            foreach (List<int> sumador in sumadores)
            {
                int[] posicionActual = [px, py];
                bool fichaEncontrada = false;
                while (!fichaEncontrada)
                {
                    posicionActual[0] += sumador[0];
                    posicionActual[1] += sumador[1];
                    if (ValidarIndiceFueraDeRango(posicionActual)) break;
                    Button botonActual = tablero[posicionActual[0], posicionActual[1]];
                    string[] fichasEnemigas = esBlanco ? negras : blancas;
                    if (botonActual.Text != "" && !(fichasEnemigas.Contains(botonActual.Text)))
                    {
                        fichaEncontrada = true;
                        break;
                    }
                    botonActual.BackColor = Color.Red;
                    if (fichasEnemigas.Contains(botonActual.Text)) {
                        fichaEncontrada = true;
                    }
                }
            }
        }
        
        private void MovimientosCaballo(int px, int py, bool esBlanco)
        {

            List<List<int>> multiplicadores = new List<List<int>>([[1, 1], [1, -1], [-1, 1], [-1, -1]]);
            List<List<int>> posiblesMovimientos = new List<List<int>>([[2, 1], [1, 2]]);
            foreach (List<int> multiplicador in multiplicadores)
            {
                foreach(List<int> movimiento in posiblesMovimientos)
                {
                    int[] posicionActual = [px, py];
                    posicionActual[0] += movimiento[0] * multiplicador[0];
                    posicionActual[1] += movimiento[1] * multiplicador[1];
                    if (ValidarIndiceFueraDeRango(posicionActual)) continue;
                    Button botonActual = tablero[posicionActual[0], posicionActual[1]];
                    string[] fichasEnemigas = esBlanco ? negras : blancas;
                    if (botonActual.Text != "" && !(fichasEnemigas.Contains(botonActual.Text))) continue;
                    botonActual.BackColor = Color.Red;
                }
            }
        }
        
        private void MovimientosRey(int px, int py, bool esBlanco)
        {
            List<int[]> movimientos = new List<int[]>([[0, -1], [-1, 0], [1, 0], [0, 1],
                                                       [- 1, - 1], [- 1, 1], [1, - 1], [1,  1]]);
            foreach(int[] movimiento in movimientos)
            {
                int[] posicionActual = [px, py];
                posicionActual[0] += movimiento[0];
                posicionActual[1] += movimiento[1];
                if (ValidarIndiceFueraDeRango(posicionActual)) continue;
                Button botonActual = tablero[posicionActual[0], posicionActual[1]];
                string[] fichasEnemigas = esBlanco ? negras : blancas;
                if (botonActual.Text != "" && !(fichasEnemigas.Contains(botonActual.Text))) continue;
                botonActual.BackColor = Color.Red;
            }
        }

        public void ValidarHayGanador()
        {
            List<string> fichasRestantesBlancas = new List<string>();
            List<string> fichasRestantesNegras = new List<string>();

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (blancas.Contains(fichas[i, j])) fichasRestantesBlancas.Add(fichas[i, j]);
                    if (negras.Contains(fichas[i, j])) fichasRestantesNegras.Add(fichas[i, j]);
                }
            }

            if (!fichasRestantesBlancas.Contains("♔"))
            {
                MessageBox.Show("Las fichas Negras han ganado");
                this.Close();
            }
            else if (!fichasRestantesNegras.Contains("♚"))
            {
                MessageBox.Show("Las fichas Blancas han ganado");
                this.Close();
            }
        }
    }
}
