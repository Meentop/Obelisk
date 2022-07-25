using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public abstract class CombatBuilding : Building, IWorkplace
{
    protected EnemyAttacks enemyAttacks;
    Cycles cycles;

    [Header("Base Characteristics")]
    [SerializeField] protected int damage;
    [SerializeField] float baseAttackSpeed; 
    protected float curAttackSpeed;
    float reloadTime;
    float curReloadTime;
    public float radius;
    [SerializeField] protected float rotateSpeed;
    [SerializeField] protected float projectileSpeed;

    [Header("Other")]
    public Transform rotationAttackPoint;
    [SerializeField] Transform personSpawnPoint;
    [SerializeField] protected GameObject projectile;
    [SerializeField] protected Transform shootPoint;
    [SerializeField] GameObject sleep;

    protected Person person = null;

    public Priority priority = Priority.First;

    protected override void Start()
    {
        base.Start();
        enemyAttacks = EnemyAttacks.Instance;
        cycles = Cycles.Instance;
        curReloadTime = 0;
    }

    [SerializeField] protected Enemy[] enemies;
    protected Enemy target;
    protected override void Update()
    {
        base.Update();
        SetEnemies();
        if (target == null || !HasTarget())
            DefineTarget();
    }

    private void FixedUpdate()
    {
        Reload();
        Rotation();
    }

    void Rotation()
    {
        if (target != null)
        {
            Vector3 direction = (target.transform.position - rotationAttackPoint.position).normalized;
            Vector3 angle = new Vector3(0, Mathf.Atan2(direction.x, direction.z) * (180 / Mathf.PI), 0);
            rotationAttackPoint.rotation = Quaternion.RotateTowards(rotationAttackPoint.rotation, Quaternion.Euler(angle), rotateSpeed * cycles.timeScale);
            SetPersonPosition();
            if(Quaternion.Angle(rotationAttackPoint.rotation, Quaternion.Euler(angle)) <= 1f && curReloadTime <= 0)
            {
                Shoot();
                curReloadTime = reloadTime;
            }
        }
    }

    protected virtual void Shoot()
    {
        if (person != null)
        {
            Projectile proj = Instantiate(projectile, shootPoint.position, Quaternion.identity).GetComponent<Projectile>();
            proj.BasicInit(target, projectileSpeed, damage);
        }
    }

    void Reload()
    {
        if (curReloadTime > 0)
            curReloadTime -= Time.fixedDeltaTime * cycles.timeScale;
    }



    
    protected virtual void SetEnemies()
    {
        if (enemyAttacks.IsEnemyAttack() && person != null)
        {
            Collider[] colliders = Physics.OverlapSphere(center.position, radius, LayerMask.GetMask("Enemy"));
            IEnumerable<Collider> coll = colliders.Where(enemy => enemy.GetComponent<Enemy>().invulnerable == false).Where(enemy => !enemy.GetComponent<Enemy>().HasMagic());
            enemies = new Enemy[coll.Count()];
            for (int i = 0; i < coll.Count(); i++)
                enemies[i] = coll.ElementAt(i).GetComponent<Enemy>();
        }
    }
    bool HasTarget()
    {
        foreach (Enemy enemy in enemies)
        {
            if (target == enemy)
                return true;
        }
        return false;
    }
    void DefineTarget()
    {
        if (enemyAttacks.IsEnemyAttack())
        {
            if (enemies.Length < 1)
            {
                target = null;
                return;
            }
            else if (priority == Priority.First)
                target = enemies.OrderBy(enemy => enemy.number).First();
            else if (priority == Priority.Last)
                target = enemies.OrderBy(enemy => enemy.number).Last(); 
            else if (priority == Priority.Weakest) 
                target = enemies.OrderBy(enemy => enemy.strength).ThenBy(enemy => enemy.number).First();
            else if (priority == Priority.Strongest)
                target = enemies.OrderBy(enemy => enemy.strength).ThenByDescending(enemy => enemy.number).Last();
            else if (priority == Priority.Nearest)
                target = enemies.OrderBy(enemy => Vector3.Distance(enemy.transform.position, center.position)).First();
            else if (priority == Priority.Random)
                target = enemies[Random.Range(0, enemies.Length)];
        }
    }


    public override void SetPosition(Vector3 pos)
    {
        base.SetPosition(pos);
        if (person != null)
            SetPersonPosition();
    }

    public override void Click()
    {
        SetCurAttackSpeed();
        ui.EnableCombatPanel(buildingsName, description, this, person, damage, curAttackSpeed, radius, rotateSpeed, projectileSpeed);
        ui.SetBuildingForPrioriting(this);
        ui.DestroyCharacteristics();
        InitCharacteristics();
    }

    public override void Destroy()
    {
        if(person != null)
            RemoveWorker(0);
        base.Destroy();
    }

    public bool HasWorkplace()
    {
        return person == null;
    }

    public void AddWorker(GameObject person)
    {
        if (this.person == null)
        {
            this.person = person.GetComponent<Person>();
            SetPersonPosition();
            person.GetComponent<Person>().workplace = this;
            this.person.inCombatBuilding = true;
            SetCurAttackSpeed();
            ui.UpdateCombatStatBlock(this, this.person, curAttackSpeed);
            sleep.SetActive(false);
            ui.DestroyCharacteristics();
            InitCharacteristics();
        }
    }

    public Person RemoveWorker(int number)
    {
        Person person = this.person;
        person.workplace = null;
        person.inCombatBuilding = false;
        person.nextPosition = new Vector3(person.transform.position.x, 0f, person.transform.position.z);
        person.transform.position = person.nextPosition;
        curAttackSpeed = 0;
        this.person = null;
        sleep.SetActive(true);
        return person;
    }

    void SetPersonPosition()
    {
        person.transform.position = personSpawnPoint.position;
        person.nextPosition = personSpawnPoint.position;
    }

    public int FindIndex(Person person)
    {
        return 0;
    }



    public void SetCurAttackSpeed()
    {
        if (person != null)
        {
            float modifier = 1;
            if (person.isHungry)
                modifier = 0.75f;
            curAttackSpeed = baseAttackSpeed * modifier;
            reloadTime = (60 / curAttackSpeed) / 60;
        }
    }

    public abstract void InitCharacteristics();
}

public enum Priority 
{ 
    First,
    Last,
    Weakest,
    Strongest,
    Nearest,
    Random
}