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
        private CameraBehavior cameraBehavior;
        private Material grayMaterial;
        private Material redMaterial;
        private Material orangeMaterial;
        private Material blueMaterial;

        public enum SneakerColor
        {
            Gray,
            Red,
            Orange,
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
            this.grayMaterial = assetsService.Load<Material>(EvergineContent.Models.AirJordan_Embedded.Materials.GrayMat);
            this.redMaterial = assetsService.Load<Material>(EvergineContent.Models.AirJordan_Embedded.Materials.RedMat);
            this.orangeMaterial = assetsService.Load<Material>(EvergineContent.Models.AirJordan_Embedded.Materials.OrangeMat);
            this.blueMaterial = assetsService.Load<Material>(EvergineContent.Models.AirJordan_Embedded.Materials.BlueMat);

            //  Select material component
            var screenContextManager = Application.Current.Container.Resolve<ScreenContextManager>();
            screenContextManager.OnActivatingScene += (scene) =>
            {
                var entity = scene.Managers.EntityManager.FindAllByTag("SneakersMesh").First();
                this.materialComponent = entity.FindComponent<MaterialComponent>();
                this.cameraBehavior = scene.Managers.EntityManager.FindComponentsOfType<CameraBehavior>().First();
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
                case SneakerColor.Orange:
                    this.materialComponent.Material = this.orangeMaterial;
                    break;
                case SneakerColor.Blue:
                    this.materialComponent.Material = this.blueMaterial;
                    break;
            }
        }
    }
}
