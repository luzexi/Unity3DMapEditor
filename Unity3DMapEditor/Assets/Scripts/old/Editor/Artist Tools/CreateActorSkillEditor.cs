using UnityEditor;
using UnityEngine;
class CreateActorSkillEditor
{
    static GameObject actorSkill = null;
    [MenuItem("SG Tools/Actor Skill")]
    static void Execute()
    {
        if (actorSkill == null)
        {
            actorSkill = new GameObject("ActorSkill");
            actorSkill.AddComponent(typeof(ActorSkill));
        }
    }
}