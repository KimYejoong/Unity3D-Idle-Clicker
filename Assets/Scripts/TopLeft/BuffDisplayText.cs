using UnityEngine;
using UnityEngine.UI;

namespace TopLeft
{
    public class BuffDisplayText : MonoBehaviour
    {
        private BuffDisplayManager _buffDisplayManager;
        public SkillButton skill;
        public Text displayText;

        public void Initialize(BuffDisplayManager manager, SkillButton skillButton)
        {
            _buffDisplayManager = manager;
            this.skill = skillButton;
            displayText = GetComponent<Text>();        

            UpdateDisplayText();
        }

        public void UpdateDisplayText()
        {
            if (skill.remaining > 0)
            {
                var hour = (int)(skill.remaining / 3600);
                var min = (int)(skill.remaining - hour * 3600) / 60;
                var sec = (int)(skill.remaining % 60);

                string timeText;
                if (hour > 0)
                    timeText = $"{hour:00}" + " : " + $"{min:00}" + " : " + $"{sec:00}";
                else
                    timeText = $"{min:00}" + " : " + $"{sec:00}";

                displayText.text = skill.skillName + " : " + "지속 시간 동안 재화 획득량 " + $"{skill.goldMultiplier:0.00}" + "배(" + timeText + ")";
            }
            else
            {
                Destroy(this.gameObject);
            }
        }

        private void OnDestroy()
        {
            _buffDisplayManager.UpdateBuffDisplayList(this);        
        }


    }
}
