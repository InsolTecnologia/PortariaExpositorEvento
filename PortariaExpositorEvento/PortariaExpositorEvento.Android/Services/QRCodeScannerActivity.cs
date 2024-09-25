using Android.App;
using Android.OS;
using Android.Content.PM;
using AndroidX.Camera.Core;
using AndroidX.Camera.View;
using AndroidX.Core.Content;
using AndroidX.Core.App;
using Android.Util;
using Android.Graphics;
using ZXing.Mobile;
using System;
using System.Linq;
using AndroidX.AppCompat.App;
using AndroidX.Camera.Lifecycle;

namespace PortariaExpositorEvento.Android.Services
{
    [Activity(Label = "QR Scanner", Theme = "@style/MyTheme.NoActionBar")]
    public class QRCodeScannerActivity : AppCompatActivity
    {
        PreviewView previewView;
        ImageAnalysis imageAnalysis;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Definir o layout da câmera
            SetContentView(Resource.Layout.activity_qr_scanner);

            previewView = FindViewById<PreviewView>(Resource.Id.previewView);

            // Verificar se a permissão de câmera foi concedida
            if (ContextCompat.CheckSelfPermission(this, Android.Manifest.Permission.Camera) != (int)Permission.Granted)
            {
                // Solicitar permissão de câmera se não foi concedida
                ActivityCompat.RequestPermissions(this, new String[] { Android.Manifest.Permission.Camera }, 0);
            }
            else
            {
                // Iniciar a câmera se a permissão já foi concedida
                StartCamera();
            }
        }

        private void StartCamera()
        {
            var cameraProviderFuture = ProcessCameraProvider.GetInstance(this);
            cameraProviderFuture.AddListener(new Java.Lang.Runnable(() =>
            {
                try
                {
                    var cameraProvider = (ProcessCameraProvider)cameraProviderFuture.Get();
                    var preview = new AndroidX.Camera.Core.Preview.Builder().Build();
                    preview.SetSurfaceProvider(previewView.SurfaceProvider);

                    // Configurando a análise de imagens para processar frames
                    imageAnalysis = new ImageAnalysis.Builder()
                        .SetTargetResolution(new Android.Util.Size(1280, 720))
                        .SetBackpressureStrategy(ImageAnalysis.StrategyKeepOnlyLatest)
                        .Build();

                    // Passar os frames para ZXing para decodificar QR Codes
                    imageAnalysis.SetAnalyzer(ContextCompat.GetMainExecutor(this), new QRCodeAnalyzer());

                    // Abrir a câmera traseira
                    var cameraSelector = new CameraSelector.Builder()
                        .RequireLensFacing(CameraSelector.LensFacingBack)
                        .Build();

                    cameraProvider.BindToLifecycle(this, cameraSelector, preview, imageAnalysis);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro ao iniciar a câmera: {ex.Message}");
                }
            }), ContextCompat.GetMainExecutor(this));
        }

        // Tratamento de resultado da permissão
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            // Verificar se a permissão foi concedida
            if (requestCode == 0 && grantResults.Length > 0 && grantResults[0] == Permission.Granted)
            {
                StartCamera(); // Iniciar a câmera após a permissão ser concedida
            }
            else
            {
                // A permissão foi negada, exibir uma mensagem ou tomar outra ação
                Console.WriteLine("Permissão de câmera negada.");
            }
        }
    }

    public class QRCodeAnalyzer : Java.Lang.Object, ImageAnalysis.IAnalyzer
    {
        public void Analyze(IImageProxy image)
        {
            // Converter o frame para byte array
            var buffer = image.GetPlanes()[0].Buffer;
            var bytes = new byte[buffer.Capacity()];
            buffer.Get(bytes);

            // Cria o ZXing BarcodeReader
            var barcodeReader = new ZXing.Mobile.BarcodeReader();

            // Processar o frame como Bitmap
            var bitmap = BitmapFactory.DecodeByteArray(bytes, 0, bytes.Length);

            if (bitmap != null)
            {
                var result = barcodeReader.Decode(bitmap);
                if (result != null)
                {
                    Console.WriteLine("QR Code Detectado: " + result.Text);
                    // Exibir ou processar o resultado conforme necessário
                }
            }

            image.Close(); // Fechar o ImageProxy
        }
    }
}
