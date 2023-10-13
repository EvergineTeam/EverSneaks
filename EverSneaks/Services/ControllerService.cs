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
        private MaterialComponent floorMaterialComponent;
        private CameraBehavior cameraBehavior;
        private Material graySneakersMaterial;
        private Material redSneakersMaterial;
        private Material orangeSneakersMaterial;
        private Material blueSneakersMaterial;

        private Material grayFloorMaterial;
        private Material blueFloorMaterial;
        private Material redFloorMaterial;
        private Material orangeFloorMaterial;

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
            this.graySneakersMaterial = assetsService.Load<Material>(EvergineContent.Models.AirJordan_Embedded.Materials.GrayMat);
            this.redSneakersMaterial = assetsService.Load<Material>(EvergineContent.Models.AirJordan_Embedded.Materials.RedMat);
            this.orangeSneakersMaterial = assetsService.Load<Material>(EvergineContent.Models.AirJordan_Embedded.Materials.OrangeMat);
            this.blueSneakersMaterial = assetsService.Load<Material>(EvergineContent.Models.AirJordan_Embedded.Materials.BlueMat);

            this.grayFloorMaterial = assetsService.Load<Material>(EvergineContent.Materials.grayFloor);
            this.blueFloorMaterial = assetsService.Load<Material>(EvergineContent.Materials.blueFloor);
            this.redFloorMaterial = assetsService.Load<Material>(EvergineContent.Materials.redFloor);
            this.orangeFloorMaterial = assetsService.Load<Material>(EvergineContent.Materials.orangeFloor);

            //  Select material component
            var screenContextManager = Application.Current.Container.Resolve<ScreenContextManager>();
            screenContextManager.OnActivatingScene += (scene) =>
            {
                var entity = scene.Managers.EntityManager.FindAllByTag("SneakersMesh").First();
                this.materialComponent = entity.FindComponent<MaterialComponent>();
                var floor = scene.Managers.EntityManager.FindAllByTag("Floor").First();
                this.floorMaterialComponent = floor.FindComponent<MaterialComponent>();
                this.cameraBehavior = scene.Managers.EntityManager.FindComponentsOfType<CameraBehavior>().First();
            };
        }

        private void UpdateSneakerColor()
        {
            switch (this.sneakerColor)
            {
                case SneakerColor.Gray:
                    this.materialComponent.Material = this.graySneakersMaterial;
                    this.floorMaterialComponent.Material = this.grayFloorMaterial;
                    break;
                case SneakerColor.Red:
                    this.materialComponent.Material = this.redSneakersMaterial;
                    this.floorMaterialComponent.Material = this.redFloorMaterial;
                    break;
                case SneakerColor.Orange:
                    this.materialComponent.Material = this.orangeSneakersMaterial;
                    this.floorMaterialComponent.Material = this.orangeFloorMaterial;
                    break;
                case SneakerColor.Blue:
                    this.materialComponent.Material = this.blueSneakersMaterial;
                    this.floorMaterialComponent.Material = this.blueFloorMaterial;
                    break;
            }
        }
    }
}
