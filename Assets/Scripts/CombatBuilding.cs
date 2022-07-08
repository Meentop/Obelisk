using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CombatBuilding : Building, IWorkplace
{
    EnemyAttacks enemyAttacks;
    Cycles cycles;

    [Header("Characteristics")]
    [SerializeField] int damage;
    [SerializeField] float baseAttackSpeed; // shoots per second
    float curAttackSpeed;
    float reloadTime;
    [SerializeField] float curReloadTime;
    public float radius;
    [SerializeField] float rotateSpeed;
    [SerializeField] float projectileSpeed;

    [Header("Other")]
    public Transform rotationAttackPoint;
    [SerializeField] Transform personSpawnPoint;
    [SerializeField] GameObject projectile;
    [SerializeField] Transform shootPoint;
    [SerializeField] GameObject sleed;

    Person person = null;

    public Priority priority = Priority.First;

    protected override void Start()
    {
        base.Start();
        enemyAttacks = EnemyAttacks.Instance;
        cycles = Cycles.Instance;
        curReloadTime = 0;
    }

    [SerializeField] Enemy[] enemies;
    [SerializeField] Enemy target;
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
            Vector3 delta = (target.transform.position - center.position).normalized;
            Vector3 direction = new Vector3(delta.x, 0, delta.z);
            rotationAttackPoint.forward = Vector3.MoveTowards(rotationAttackPoint.forward, direction, rotateSpeed * cycles.timeScale);
            if(Vector3.Distance(direction, rotationAttackPoint.forward) < 0.0001f && curReloadTime <= 0)
            {
                Shoot();
                curReloadTime = reloadTime;
            }
        }
    }

    void Shoot()
    {
        if (person != null)
        {
            Projectile proj = Instantiate(projectile, shootPoint.position, Quaternion.identity).GetComponent<Projectile>();
            proj.Inisialization(target, projectileSpeed, damage);
        }
    }

    void Reload()
    {
        if (curReloadTime > 0)
            curReloadTime -= Time.fixedDeltaTime * cycles.timeScale;
    }



    void SetEnemies()
    {
        if (enemyAttacks.IsEnemyAttack() && person != null)
        {
            Collider[] colliders = Physics.OverlapSphere(center.position, radius, LayerMask.GetMask("Enemy"));
            enemies = new Enemy[colliders.Length];
            for (int i = 0; i < colliders.Length; i++)
                enemies[i] = colliders[i].GetComponent<Enemy>();
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
        if (enemies.Length < 1)
        {
            target = null;
            return;
        }
        if(priority == Priority.First)
        {
            int minNumber = 100;
            Enemy first = enemies[0];
            foreach (Enemy enemy in enemies)
            {
                if (enemy.number < minNumber)
                {
                    first = enemy;
                    minNumber = enemy.number;
                }
            }
            target = first;
        }
        else if(priority == Priority.Last)
        {
            int maxNumber = 0;
            Enemy last = enemies[0];
            foreach (Enemy enemy in enemies)
            {
                if (enemy.number > maxNumber)
                {
                    last = enemy;
                    maxNumber = enemy.number;
                }
            }
            target = last;
        }
        else if(priority == Priority.Weakest)
        {
            int minStrength = 100;
            Enemy weakest = enemies[0];
            foreach (Enemy enemy in enemies)
            {
                if (enemy.strength < minStrength)
                {
                    weakest = enemy;
                    minStrength = enemy.strength;
                }
            }
            target = weakest;
        }
        else if (priority == Priority.Strongest)
        {
            int maxStrength = 100;
            Enemy stronger = enemies[0];
            foreach (Enemy enemy in enemies)
            {
                if (enemy.strength > maxStrength)
                {
                    stronger = enemy;
                    maxStrength = enemy.strength;
                }
            }
            target = stronger;
        }
        else if(priority == Priority.Nearest)
        {
            float minDistance = 100;
            Enemy nearest = enemies[0];
            foreach (Enemy enemy in enemies)
            {
                if (Vector3.Distance(enemy.transform.position, center.position) < minDistance)
                {
                    nearest = enemy;
                    minDistance = Vector3.Distance(enemy.transform.position, center.position);
                }
            }
            target = nearest;
        }
        else if(priority == Priority.Random)
            target = enemies[Random.Range(0, enemies.Length)];
    }




    public override void SetPosition(Vector3 pos)
    {
        base.SetPosition(pos);
        if (person != null)
        {
            person.transform.position = personSpawnPoint.position;
            person.nextPosition = personSpawnPoint.position;
        }
    }

    public override void Click()
    {
        //before enemies attack set cur speed attack
        SetCurAttackSpeed();
        ui.EnableCombatPanel(buildingsName, this, person, damage, curAttackSpeed, radius, rotateSpeed, projectileSpeed);
        ui.SetBuildingForPriority(this);
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
            person.transform.position = personSpawnPoint.position;
            this.person.nextPosition = personSpawnPoint.position;
            person.GetComponent<Person>().workplace = this;
            this.person.inCombatBuilding = true;
            SetCurAttackSpeed();
            ui.UpdateCombatStatBlock(this, this.person, curAttackSpeed);
            sleed.SetActive(false);
        }
    }

    public Person RemoveWorker(int number)
    {
        Person person = this.person;
        person.workplace = null;
        person.gameObject.SetActive(true);
        person.inCombatBuilding = false;
        curAttackSpeed = 0;
        this.person = null;
        sleed.SetActive(true);
        return person;
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