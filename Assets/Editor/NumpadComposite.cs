using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.Scripting;

#if UNITY_EDITOR
namespace Editor
{
    [UnityEditor.InitializeOnLoad]
#endif
    [Preserve]
    [DisplayStringFormat("{up}/{left}/{down}/{right}/{upleft}/{upright}/{downleft}/{downright}")]
    // ReSharper disable once InconsistentNaming
    public class NumpadComposite : InputBindingComposite<Vector2> {
 
        // NOTE: This is a modified copy of Vector2Composite
        // (Source: https://forum.unity.com/threads/using-last-key-pressed-in-a-composite-2d-vector.1109609/) 
 
        [InputControl(layout = "Button")]
        public int Up = 0;
        [InputControl(layout = "Button")]
        public int Down = 0;
        [InputControl(layout = "Button")]
        public int Left = 0;
        [InputControl(layout = "Button")]
        public int Right = 0;
        [InputControl(layout = "Button")]
        public int UpLeft = 0;
        [InputControl(layout = "Button")]
        public int UpRight = 0;
        [InputControl(layout = "Button")]
        public int DownLeft = 0;
        [InputControl(layout = "Button")]
        public int DownRight = 0;
 
        
        public override Vector2 ReadValue(ref InputBindingCompositeContext context) {
            var upPressed    = context.ReadValueAsButton(Up);
            var downPressed  = context.ReadValueAsButton(Down);
            var leftPressed  = context.ReadValueAsButton(Left);
            var rightPressed = context.ReadValueAsButton(Right);
            var upLeftPressed = context.ReadValueAsButton(UpLeft);
            var upRightPressed = context.ReadValueAsButton(UpRight);
            var downLeftPressed = context.ReadValueAsButton(DownLeft);
            var downRightPressed = context.ReadValueAsButton(DownRight);
            
            var x = (leftPressed, rightPressed) switch {
                (false, false)                                              =>  0f,
                (true,  false)                                              => -1f,
                (false, true)                                               =>  1f,
                (true,  true)                                               =>  0f
            };
 
            var y = (downPressed, upPressed) switch {
                (false, false)                                           =>  0f,
                (true,  false)                                           => -1f,
                (false, true)                                            =>  1f,
                (true,  true)                                            =>  0f
            };


            var input = new Vector2(x, y);

            input = (upLeftPressed, upRightPressed) switch
            {
                (false, false) => input,
                (true, false) => new Vector2(-1, 1),
                (false, true) => new Vector2(1, 1),
                (true, true) => Vector2.zero
            };
            
            input = (downLeftPressed, downRightPressed) switch
            {
                (false, false) => input,
                (true, false) => new Vector2(-1, -1),
                (false, true) => new Vector2(1, -1),
                (true, true) => Vector2.zero
            };
 
            return input;
        }
 
        public override float EvaluateMagnitude(ref InputBindingCompositeContext context) {
            var value = ReadValue(ref context);
            return value.magnitude;
        }
 
#if UNITY_EDITOR
        static NumpadComposite() {
            Initialize();
        }
#endif
 
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Initialize() {
            InputSystem.RegisterBindingComposite<NumpadComposite>();
        }
    }
}