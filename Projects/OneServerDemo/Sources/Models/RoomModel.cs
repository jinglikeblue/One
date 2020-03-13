using One;

namespace OneServer
{
    class RoomModel : BaseModel
    {
        RoomVO _vo = new RoomVO();

        public void AddRole(RoleVO role)
        {
            _vo.roles.Add(role);
        }

        public void RemoveRole(RoleVO role)
        {
            _vo.roles.Remove(role);
        }

        public RoleVO FindRole(int id)
        {
            foreach(var role in _vo.roles)
            {
                if(role.id == id)
                {
                    return role;
                }
            }
            return null;
        }

        protected override void Reset()
        {
            
        }        
    }
}
