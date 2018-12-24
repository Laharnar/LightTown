using UnityEngine;

namespace Fantasy
{
    public class PlayerMeleeCombatControler : MeleeCombat
    {
        public override AttackSetID ReceiveAtkCommands()
        {
            if (Input.GetMouseButtonDown(0))
            {
                return AttackSetID.Normal;
            }
            else if (Input.GetMouseButtonDown(1))
            {
                return AttackSetID.Special;
            }
            else if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                return AttackSetID.Dash;
            }
            return AttackSetID.None;
        }
    }
}
