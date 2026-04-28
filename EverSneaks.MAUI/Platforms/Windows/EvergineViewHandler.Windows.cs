using Evergine.Common.Graphics;
using Evergine.DirectX11;
using Evergine.Framework.Graphics;
using Evergine.Framework.Services;
using Evergine.WinUI;
using Microsoft.Maui.Handlers;
using Microsoft.UI.Xaml.Controls;
using System.Diagnostics;
using WinUIGrid = Microsoft.UI.Xaml.Controls.Grid;

namespace EverSneaks.MAUI.Evergine
{
    public partial class EvergineViewHandler : ViewHandler<EvergineView, WinUIGrid>
    {
        private bool isViewLoaded;

        private SwapChainPanel swapChainPanel;

        private WinUIWindowsSystem windowsSystem;
        private static bool isEvergineInitialized;
        private static DX11GraphicsContext graphicsContext;
        private static SwapChain swapChain;

        public EvergineViewHandler(IPropertyMapper mapper, CommandMapper commandMapper = null)
            : base(mapper, commandMapper)
        {
        }

        public static void MapApplication(EvergineViewHandler handler, EvergineView evergineView)
        {
            if (!handler.isViewLoaded)
            {
                return;
            }

            handler.UpdateApplication(handler.swapChainPanel, evergineView, evergineView.DisplayName);
        }

        protected override WinUIGrid CreatePlatformView()
        {
            var platformView = new WinUIGrid();

            this.swapChainPanel = new SwapChainPanel
            {
                IsHitTestVisible = true,
            };

            platformView.Children.Add(this.swapChainPanel);

            return platformView;
        }

        protected override void ConnectHandler(WinUIGrid platformView)
        {
            base.ConnectHandler(platformView);

            this.isViewLoaded = false;

            platformView.Loaded += this.OnPlatformViewLoaded;

            this.swapChainPanel.PointerPressed += this.OnPlatformViewPointerPressed;
            this.swapChainPanel.PointerMoved += this.OnPlatformViewPointerMoved;
            this.swapChainPanel.PointerReleased += this.OnPlatformViewPointerReleased;
            this.swapChainPanel.SizeChanged += this.OnSwapChainPanelSizeChanged;
        }

        protected override void DisconnectHandler(WinUIGrid platformView)
        {
            base.DisconnectHandler(platformView);

            platformView.Loaded -= this.OnPlatformViewLoaded;

            this.swapChainPanel.PointerPressed -= this.OnPlatformViewPointerPressed;
            this.swapChainPanel.PointerMoved -= this.OnPlatformViewPointerMoved;
            this.swapChainPanel.PointerReleased -= this.OnPlatformViewPointerReleased;
            this.swapChainPanel.SizeChanged -= this.OnSwapChainPanelSizeChanged;
        }

        private void OnPlatformViewLoaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            this.isViewLoaded = true;
            this.UpdateValue(nameof(EvergineView.Application));
        }

        private void OnPlatformViewPointerReleased(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            this.VirtualView.StartInteraction();
        }

        private void OnPlatformViewPointerMoved(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            this.VirtualView.MovedInteraction();
        }

        private void OnPlatformViewPointerPressed(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            this.VirtualView.EndInteraction();
        }

        private void OnSwapChainPanelSizeChanged(object sender, Microsoft.UI.Xaml.SizeChangedEventArgs e)
        {
            if (e.NewSize.Width <= 0 || e.NewSize.Height <= 0)
            {
                return;
            }

            swapChain?.ResizeSwapChain((uint)e.NewSize.Width, (uint)e.NewSize.Height);
        }

        private void UpdateApplication(SwapChainPanel swapChainPanel, EvergineView view, string displayName)
        {
            if (view.Application is null)
            {
                return;
            }

            if (this.windowsSystem == null)
            {
                this.windowsSystem = new WinUIWindowsSystem();
            }

            if (!isEvergineInitialized)
            {
                view.Application.Container.RegisterInstance(this.windowsSystem);
            }
            else
            {
                view.Application.Container.Unregister<WinUIWindowsSystem>();
                view.Application.Container.RegisterInstance(this.windowsSystem);
            }

            var surface = (WinUISurface)this.windowsSystem.CreateSurface(swapChainPanel);

            var clockTimer = Stopwatch.StartNew();
            this.windowsSystem.Run(
                () =>
                {
                    this.ConfigureGraphicsContext(view.Application, surface, displayName);

                    if (!isEvergineInitialized)
                    {
                        view.Application.Initialize();
                        isEvergineInitialized = true;
                    }
                },
                () =>
                {
                    var gameTime = clockTimer.Elapsed;
                    clockTimer.Restart();

                    view.Application.UpdateFrame(gameTime);
                    view.Application.DrawFrame(gameTime);
                });
        }

        private void ConfigureGraphicsContext(global::Evergine.Framework.Application application, WinUISurface surface, string displayName)
        {
            if (graphicsContext == null)
            {
                graphicsContext = new DX11GraphicsContext();
                graphicsContext.CreateDevice();
            }

            var swapChainDescription = new SwapChainDescription()
            {
                SurfaceInfo = surface.SurfaceInfo,
                Width = surface.Width,
                Height = surface.Height,
                ColorTargetFormat = PixelFormat.R8G8B8A8_UNorm,
                ColorTargetFlags = TextureFlags.RenderTarget | TextureFlags.ShaderResource,
                DepthStencilTargetFormat = PixelFormat.D24_UNorm_S8_UInt,
                DepthStencilTargetFlags = TextureFlags.DepthStencil,
                SampleCount = TextureSampleCount.None,
                IsWindowed = true,
                RefreshRate = 60,
            };

            displayName = string.IsNullOrWhiteSpace(displayName) ? "DefaultDisplay" : displayName;

            swapChain = graphicsContext.CreateSwapChain(swapChainDescription);
            swapChain.VerticalSync = true;
            surface.NativeSurface.SwapChain = swapChain;

            var graphicsPresenter = application.Container.Resolve<GraphicsPresenter>();
            var firstDisplay = new Display(surface, swapChain);

            if (!isEvergineInitialized)
            {
                graphicsPresenter.AddDisplay(displayName, firstDisplay);
                application.Container.RegisterInstance(graphicsContext);
            }
            else
            {
                graphicsPresenter.RemoveDisplay(displayName);
                graphicsPresenter.AddDisplay(displayName, firstDisplay);
            }
        }
    }
}
