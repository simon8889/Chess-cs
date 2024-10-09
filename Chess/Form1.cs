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
            lbl_info.Size = new Size(200, 20);
            lbl_info.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            MostrarInfoFichas();
            this.Controls.Add(lbl_info);
            
            lbl_tiempoBlancas.Location = new Point(0, celda * 8 + 20);
            lbl_tiempoBlancas.Size = new Size(200, 20);
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
            if (tiempoRestanteNegras == 0 || tiempoRestanteBlancas == 0)
            {
                string ganador = tiempoRestanteBlancas == 0 ? "Negras" : "Blancas";
                cronometro.Stop();
                MessageBox.Show($"Se ha acabado el tiempo, Las fichas {ganador} han ganado");
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
            PintarMovimientos(px, py);
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

        private void PintarMovimientos(int px, int py)
        {
            List<int[]> movimientos = ObtenerListaDeMovimientos(px, py);
            PintarListaDeMovimientos(movimientos);
        }
        private void PintarListaDeMovimientos(List<int[]> movimientos)
        {
            foreach (int[] movimiento in movimientos)
            {
                tablero[movimiento[0], movimiento[1]].BackColor = Color.Red;
            }
        }
        
        private List<int[]> ObtenerListaDeMovimientos(int px, int py)
        {
            string ficha = fichas[px, py];
            switch (ficha)
            {
                case "♙" or "♟":
                    return MovimientosFichas.ListaMovimientosPeon(px, py, turnoBlanca, fichas);
                case "♗" or "♝":
                    return MovimientosFichas.ListaMovimientosAlfil(px, py, turnoBlanca, fichas);
                case "♜" or "♖":
                    return MovimientosFichas.ListaMovimientosTorre(px, py, turnoBlanca, fichas);
                case "♘" or "♞":
                    return MovimientosFichas.ListaMovimientosCaballo(px, py, turnoBlanca, fichas);
                case "♚" or "♔":
                    return MovimientosFichas.ListaMovimientosRey(px, py, turnoBlanca, fichas);
                case "♛" or "♕":
                    return MovimientosFichas.ListaMovimientosReina(px, py, turnoBlanca, fichas);
                default:
                    return new List<int[]>(); 
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
