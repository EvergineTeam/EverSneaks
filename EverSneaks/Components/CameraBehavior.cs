// Copyright © Plain Concepts S.L.U. All rights reserved. Use is subject to license terms.

using Evergine.Common.Graphics;
using Evergine.Common.Input;
using Evergine.Common.Input.Mouse;
using Evergine.Common.Input.Pointer;
using Evergine.Components.WorkActions;
using Evergine.Framework;
using Evergine.Framework.Graphics;
using Evergine.Framework.Services;
using Evergine.Mathematics;
using System;
using System.Linq;
using static EverSneaks.Services.ControllerService;

namespace EverSneaks.Components
{
    public class CameraBehavior : Behavior
    {
        /// <summary>
        /// The camera to move.
        /// </summary>
        [BindComponent(false)]
        public Transform3D Transform = null;

        /// <summary>
        /// The child transform.
        /// </summary>
        private Transform3D childTransform;

        /// <summary>
        /// The camera transform.
        /// </summary>
        private Transform3D cameraTransform;

        /// <summary>
        /// True if the camera Is Dragging.
        /// </summary>
        private bool isRotating;

        /// <summary>
        /// The orbit_scale.
        /// </summary>
        private const float OrbitScale = 0.005f;

        /// <summary>
        /// Gets or sets the touch sensibility.
        /// </summary>
        /// <remarks>
        /// 0.5 is for stop, 1 is for raw delta, 2 is twice delta.
        /// </remarks>
        public float TouchSensibility { get; set; } = 0.5f;

        /// <summary>
        /// The theta.
        /// </summary>
        private float theta;

        /// <summary>
        /// The is dirty.
        /// </summary>
        private bool isDirty;

        /// <summary>
        /// The current mouse state.
        /// </summary>
        private Vector3 cameraInitialPosition;

        private MouseDispatcher mouseDispatcher;
        private Evergine.Mathematics.Point currentMouseState;
        private Vector2 lastMousePosition;

        private PointerDispatcher touchDispatcher;
        private Evergine.Mathematics.Point currentTouchState;
        private Vector2 lastTouchPosition;
        private IWorkAction animation;
        private bool animating;
        private float initialRotationY;

        //private readonly Color violetColor = new Color("#6F64EDFF");
        //private readonly Color blueColor = new Color("#2EB6FDFF");
        //private readonly Color pinkColor = new Color("#F92EFDFF");
        //private readonly Color orangeColor = new Color("#FD9D2EFF");

        private readonly Color color1 = new Color("#e6e5e5");
        private readonly Color color2 = new Color("#b1252e");
        private readonly Color color3 = new Color("#e38210");
        private readonly Color color4 = new Color("#026da7");

        public CameraBehavior()
        {
        }

        /// <inheritdoc/>
        protected override void OnLoaded()
        {
            base.OnLoaded();

            this.theta = MathHelper.ToRadians(45);

            this.isRotating = false;

            this.isDirty = true;
        }

        /// <inheritdoc/>
        protected override bool OnAttached()
        {
            var child = this.Owner.ChildEntities.First();
            this.childTransform = child.FindComponent<Transform3D>();
            this.cameraTransform = child.ChildEntities.First().FindComponent<Transform3D>();

            this.cameraInitialPosition = this.cameraTransform.LocalPosition;

            return base.OnAttached();
        }

        /// <inheritdoc/>
        protected override void OnActivated()
        {
            base.OnActivated();

            var display = this.Owner.Scene.Managers.RenderManager.ActiveCamera3D?.Display;
            if (display != null)
            {
                this.mouseDispatcher = display.MouseDispatcher;
                this.touchDispatcher = display.TouchDispatcher;
            }
        }

        /// <inheritdoc/>
        protected override void Update(TimeSpan gameTime)
        {
            this.HandleInput();

            if (this.isDirty)
            {
                this.CommitChanges();
                this.isDirty = false;
            }
        }

        /// <summary>
        /// Handle the Mouse and Pointer events
        /// </summary>
        private void HandleInput()
        {
            if (this.animating)
            {
                return;
            }

            if (Evergine.Platform.DeviceInfo.PlatformType == Evergine.Common.PlatformType.Windows)
            {
                this.HandleMouse();
            }
            else
            {
                this.HandleTouch();
            }
        }

        /// <summary>
        /// Handles the mouse events.
        /// </summary>
        private void HandleMouse()
        {
            if (this.mouseDispatcher == null)
            {
                return;
            }

            // Orbit            
            if (this.mouseDispatcher.IsButtonDown(MouseButtons.Left))
            {
                this.currentMouseState = this.mouseDispatcher.Position;

                if (this.isRotating == false)
                {
                    this.isRotating = true;
                }
                else
                {
                    Vector2 delta = Vector2.Zero;

                    delta.X = -this.currentMouseState.X + this.lastMousePosition.X;
                    delta.Y = this.currentMouseState.Y - this.lastMousePosition.Y;

                    delta = -delta;
                    this.Orbit(delta * OrbitScale);
                }

                this.lastMousePosition.X = this.currentMouseState.X;
                this.lastMousePosition.Y = this.currentMouseState.Y;
            }
            else
            {
                this.isRotating = false;
            }
        }

        /// <summary>
        /// Handle the pointer events
        /// </summary>
        private void HandleTouch()
        {
            if (this.touchDispatcher == null)
            {
                return;
            }

            var point = this.touchDispatcher.Points.FirstOrDefault();

            if (point == null)
            {
                return;
            }

            if (point.State == ButtonState.Pressed)
            {
                this.currentTouchState = point.Position;

                if (this.isRotating == false)
                {
                    this.isRotating = true;
                }
                else
                {
                    Vector2 delta = Vector2.Zero;
                    delta.X = -this.currentTouchState.X + this.lastTouchPosition.X;
                    delta.Y = this.currentTouchState.Y - this.lastTouchPosition.Y;

                    delta = -delta;
                    this.Orbit(delta * OrbitScale);
                }

                this.lastTouchPosition.X = this.currentTouchState.X;
                this.lastTouchPosition.Y = this.currentTouchState.Y;
            }
            else
            {
                this.isRotating = false;
            }
        }

        /// <summary>
        /// Orbits the specified delta.
        /// </summary>
        /// <param name="delta">The delta.</param>
        public void Orbit(Vector2 delta)
        {
            this.theta += delta.X;

            this.isDirty = true;
        }

        /// <summary>
        /// Commits the changes.
        /// </summary>
        public void CommitChanges()
        {
            var rotation = this.Transform.LocalRotation;
            rotation.Y = -this.theta;

            this.Transform.LocalRotation = rotation;
        }

        /// <summary>
        /// Reset camera position.
        /// </summary>
        public void Reset()
        {
            this.cameraTransform.LocalPosition = this.cameraInitialPosition;
            this.cameraTransform.LocalLookAt(Vector3.Zero, Vector3.Up);
            this.childTransform.LocalPosition = Vector3.Zero;
            this.childTransform.LocalRotation = Vector3.Zero;
            this.Transform.LocalPosition = Vector3.Zero;
            this.Transform.LocalRotation = Vector3.Zero;

            this.theta = 0;

            this.isRotating = false;
        }

        public void PlaySpinAnimation(SneakerColor sneakerColor)
        {
            this.animating = true;
            this.animation?.Cancel();

            this.animation = this.Owner.Scene.CreateParallelWorkActions(
                    new ActionWorkAction(() => this.initialRotationY = this.Transform.LocalRotation.Y)
                                        .ContinueWith(new FloatAnimationWorkAction(this.Owner, 0, 1, TimeSpan.FromSeconds(1.0f), EaseFunction.ExponentialOutEase, (f) =>
                                        {
                                            var rotation = this.Transform.LocalRotation;
                                            rotation.Y = this.initialRotationY + (2 * (MathHelper.Pi * f));
                                            this.Transform.LocalRotation = rotation;
                                        }).ContinueWithAction(() => this.animating = false))
                    , new WaitWorkAction(TimeSpan.FromSeconds(0.2f))
                    .ContinueWithAction(() =>
                    {
                        var camera = this.Managers.RenderManager.ActiveCamera3D;
                        switch (sneakerColor)
                        {
                            case SneakerColor.Gray:                                
                                camera.BackgroundColor = color1;
                                break;
                            case SneakerColor.Red:                                
                                camera.BackgroundColor = color2;
                                break;
                            case SneakerColor.Orange:                                
                                camera.BackgroundColor = color3;
                                break;
                            case SneakerColor.Blue:                                
                                camera.BackgroundColor = color4;
                                break;
                        }
                    })
                    ).WaitAll();

            this.animation.Run();
        }
    }
}
