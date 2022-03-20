using UnityEngine;

namespace MarblePhysics.Modding.StandardComponents
{
    public class StandardComponentMetadata : MonoBehaviour
    {
        public string Name = default;
        [TextArea(1, 10)]
        public string Description = default;
        public string[] Tags = default;

        private void Awake()
        {
            this.enabled = false;
        }
    }
}