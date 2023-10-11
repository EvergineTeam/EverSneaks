using Evergine.Common.Graphics;
using Evergine.Components.Graphics3D;
using Evergine.Framework;
using Evergine.Framework.Graphics;
using Evergine.Framework.Services;
using EverSneaks.Components;
using System.Linq;

namespace EverSneaks.Services
{
    public class ControllerService : Service
    {
        private MaterialComponent materialComponent;
        private Camera3D camera;
        private CameraBehavior cameraBehavior;
        private Material grayMaterial;
        private Material redMaterial;
        private Material greenMaterial;
        private Material blueMaterial;

        public enum SneakerColor
        {
            Gray,
            Red,
            Green,
            Blue,
        };

        private SneakerColor sneakerColor;

        public SneakerColor Color
        {
            get => this.sneakerColor;
            set
            {
                this.sneakerColor = value;
                this.cameraBehavior.PlaySpinAnimation(this.sneakerColor);
                this.UpdateSneakerColor();
            }
        }

        protected override void Start()
        {
            base.Start();

            // Load materials
            var assetsService = Application.Current.Container.Resolve<AssetsService>();
            this.grayMaterial = assetsService.Load<Material>(EvergineContent.Models.sneakers.Materials.GrayMat);
            this.redMaterial = assetsService.Load<Material>(EvergineContent.Models.sneakers.Materials.RedMat);
            this.greenMaterial = assetsService.Load<Material>(EvergineContent.Models.sneakers.Materials.GreenMat);
            this.blueMaterial = assetsService.Load<Material>(EvergineContent.Models.sneakers.Materials.BlueMat);

            //  Select material component
            var screenContextManager = Application.Current.Container.Resolve<ScreenContextManager>();
            screenContextManager.OnActivatingScene += (scene) =>
            {
                var entity = scene.Managers.EntityManager.FindAllByTag("SneakersMesh").First();
                this.materialComponent = entity.FindComponent<MaterialComponent>();
                this.cameraBehavior = scene.Managers.EntityManager.FindComponentsOfType<CameraBehavior>().First();
                ////this.cameraBehavior.AnimationEvent += this.UpdateSneakerColor();
            };
        }

        private void UpdateSneakerColor()
        {
            switch (this.sneakerColor)
            {
                case SneakerColor.Gray:
                    this.materialComponent.Material = this.grayMaterial;
                    break;
                case SneakerColor.Red:
                    this.materialComponent.Material = this.redMaterial;
                    break;
                case SneakerColor.Green:
                    this.materialComponent.Material = this.greenMaterial;
                    break;
                case SneakerColor.Blue:
                    this.materialComponent.Material = this.blueMaterial;
                    break;
            }
        }
    }
}
