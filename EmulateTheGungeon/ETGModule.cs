using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public abstract class ETGModule
{
    public virtual ETGModuleMetadata Metadata { get; set; }
    public abstract void Init();
    public abstract void Start();

    [Obsolete("Add your own MonoBehaviour to the ETGModMainBehaviour.Instance.gameObject!")]
    public virtual void Update() { }
    public abstract void Exit();

}
