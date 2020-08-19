using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace HorizontalGauge
{
    public partial class FillerGauge : UserControl
    {
        public FillerGauge()
        {
            InitializeComponent();
        }

        //Declaration
        private float _max = 100;
        private float _min = 0;
        private float _valeur = 0;
        private int _noOfDivisions = 4;
        private int _noOfSubDivisions = 2;
        private int _noOfSubSubDivisions = 2;
        private Color _textColor = Color.Black;
        private Color _linesColor = Color.DimGray;
        private String _police = "Arial";
        private int _taillePolice = 26;

        [Browsable(true), Category("Properties")]
        public float Max
        {
            get { return _max; }
            set
            {
                _max = value;
                Invalidate();
            }
        }

        [Browsable(true), Category("Properties")]
        public float Min
        {
            get { return _min; }
            set
            {
                _min = value;
                Invalidate();
            }
        }

        [Browsable(true), Category("Properties")]
        public float Valeur
        {
            get { return _valeur; }
            set
            {
                if (value > Max)
                    _valeur = Max;
                else if (value < Min)
                    _valeur = Min;
                else
                    _valeur = value;

                Invalidate();
            }
        }

        [Browsable(true), Category("Properties")]
        public int NoOfDivisions
        {
            get { return _noOfDivisions; }
            set
            {
                _noOfDivisions = value;
                Invalidate();

            }
        }

        [Browsable(true), Category("Properties")]
        public int NoOfSubDivisions
        {
            get { return _noOfSubDivisions; }
            set
            {
                _noOfSubDivisions = value;
                Invalidate();

            }
        }

        [Browsable(true), Category("Properties")]
        public int NoOfSubSubDivisions
        {
            get { return _noOfSubSubDivisions; }
            set
            {
                _noOfSubSubDivisions = value;
                Invalidate();

            }
        }
        [Browsable(true), Category("Properties")]
        public Color TextColor
        {
            get { return _textColor; }
            set
            {
                _textColor = value;
                Invalidate();

            }
        }

        [Browsable(true), Category("Properties")]
        public Color LinesColor
        {
            get { return _linesColor; }
            set
            {
                _linesColor = value;
                Invalidate();

            }
        }

        [Browsable(true), Category("Properties")]
        public String Police
        {
            get { return _police; }
            set
            {
                _police = value;
                Invalidate();

            }
        }

        [Browsable(true), Category("Properties")]
        public int TaillePolice
        {
            get { return _taillePolice; }
            set
            {
                _taillePolice = value;
                Invalidate();

            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            //base.OnPaint(e);
            Graphics graphics = e.Graphics;

            // dessiner la forme d'arrière-plan
            float hauteur1 = .9f * Height;
            float largeur1 = .9f * Width;
            float x1 = .05f * Width;
            float y1 = .05f * Height;
            float radius = .1f * hauteur1;

            LinearGradientBrush bgBrush = new LinearGradientBrush(
                new PointF(x1, y1),
                new PointF(x1, y1 + hauteur1),
                Color.FromArgb(230, 230, 230),
                Color.FromArgb(200, 200, 230)
                );
            float[] factors = { .1f, .5f, .7f, .9f, 1.0f, 1.0f };
            float[] positions = { 0.0f, .2f, .4f, .6f, .8f, 1.0f };
            Blend blend = new Blend();
            blend.Factors = factors;
            blend.Positions = positions;
            bgBrush.Blend = blend;

            RectangleF rectangle = new RectangleF(x1, y1, largeur1, hauteur1);
            GraphicsPath path = this.GetRoundedRect(rectangle, radius);
            graphics.FillPath(bgBrush, path);

            // Dessiner le rectangle du jauge
            Pen penThin = new Pen(LinesColor);

            float x2 = x1 + 0.09f * largeur1;
            float y2 = y1 + (.19f + .03f) * hauteur1;
            float largeur2 = (1 - (.09f + .17f)) * largeur1;
            float hauteur2 = .22f * hauteur1;
            graphics.DrawRectangle(penThin, x2, y2, largeur2, hauteur2);

            // dessiner les petites échelles
            int NoOfDivisionsTotales = NoOfDivisions * NoOfSubDivisions * NoOfSubSubDivisions;
            float px = x2;
            float py = y2 + .02f * hauteur1;
            float hauteurLigne = hauteur2 - .04f * hauteur1;
            DrawScaleLines(graphics, penThin, px, py, hauteurLigne, NoOfDivisionsTotales, largeur2);

            Pen penThick = new Pen(LinesColor, largeur1 * .01f);
            // dessiner les moyennes échelles
            NoOfDivisionsTotales = NoOfDivisions * NoOfSubDivisions;
            px = x2;
            py = y2;
            hauteurLigne = hauteur2;
            DrawScaleLines(graphics, penThick, px, py, hauteurLigne, NoOfDivisionsTotales, largeur2);

            // dessiner les grandes échelles
            NoOfDivisionsTotales = NoOfDivisions;
            px = x2;
            py = y2 - (.05f * hauteur1);
            hauteurLigne = .35f * hauteur1;
            penThick.StartCap = LineCap.Round;
            penThick.EndCap = LineCap.Round;
            DrawScaleLines(graphics, penThick, px, py, hauteurLigne, NoOfDivisionsTotales, largeur2);

            // Dessiner la partie roulante du jauge
            LinearGradientBrush gaugeBrush = new LinearGradientBrush(
                new PointF(x2, y2),
                new PointF(x2, y1 + hauteur1),
                Color.Gold,
                Color.Red
                //Color.FromArgb(255,255, 210, 0),
                //Color.FromArgb(255,255, 0, 0)
                );
            gaugeBrush.Blend = blend;

            float largeurV = (Valeur - Min) * largeur2 / (Max - Min);
            graphics.FillRectangle(gaugeBrush, x2, y2, largeurV, hauteur2);

            // Écrire les valuer sur le jauge.
            DrawScaleTexts(graphics, x2, y2, NoOfDivisionsTotales, largeur2);

        }

        // Méthode pour dessiner un rectangle aux coins arrondis (à l'aide de: 
        // https://www.codeproject.com/Articles/5649/Extended-Graphics-An-implementation-of-Rounded-Rec)

        private GraphicsPath GetRoundedRect(RectangleF baseRect, float radius)
        {
            float diameter = radius * 2.0F;
            SizeF sizeF = new SizeF(diameter, diameter);
            RectangleF arc = new RectangleF(baseRect.Location, sizeF);
            GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath();

            path.AddArc(arc, 180, 90);

            arc.X = baseRect.Right - diameter;
            path.AddArc(arc, 270, 90);

            arc.Y = baseRect.Bottom - diameter;
            path.AddArc(arc, 0, 90);

            arc.X = baseRect.Left;
            path.AddArc(arc, 90, 90);

            path.CloseFigure();
            return path;
        }

        // Méthode pour dessiner les lignes verticales.
        private void DrawScaleLines(Graphics graphics, Pen pen, float x, float y, float hauteurLigne,
            int NoOfDivisionsTotales, float largeurTotal)
        {
            for (int i = 0; i <= NoOfDivisionsTotales; i++)
            {
                float x1 = x + (i * (largeurTotal / NoOfDivisionsTotales));
                float y1 = y;
                float x2 = x1;
                float y2 = y + hauteurLigne;
                graphics.DrawLine(pen, x1, y1, x2, y2);
            }
        }

        // Méthode pour écrire les valuer sur le jauge.
        private void DrawScaleTexts(Graphics graphics, float x, float y, int noOfDivisionsTotales,
            float largeur)
        {
            float scaleValuer;
            float maxScaleLength = Convert.ToString(Max).Length;
            Font font = new Font(Police, TaillePolice, FontStyle.Bold);

            for (int i = 0; i <= noOfDivisionsTotales; i++)
            {
                scaleValuer = Min + i * ((Max - Min) / noOfDivisionsTotales);
                String scaleText = Convert.ToString(scaleValuer);
                //Positionner le texte
                float x1 = x - (scaleText.Length / 2.5f * TaillePolice) + (i * (largeur / noOfDivisionsTotales));
                float y1 = y * 2.7f;
                
                TextRenderer.DrawText(graphics, scaleText, font,
                    new Point((int)x1, (int)y1), TextColor);
            }
        }

        private void FillerGauge_Load(object sender, EventArgs e)
        {

        }
    }
}
