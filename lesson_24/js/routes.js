async function loadStudents() {

    ClearPage();
    const table = document.getElementById('studentsTable');
    table.style = "display: flex;";

    const tbody = document.getElementById('studentBody');
    tbody.innerHTML = '';

    const res = await fetch('/students/all');

    const students = await res.json();

    for (const s of students) {
        const tr = document.createElement('tr');
        tr.innerHTML = `
                    <td>${s.studentId}</td>
                    <td>${s.firstName}</td>
                    <td>${s.lastName}</td>
                    <td>${s.age}</td>`;
        tbody.appendChild(tr);
    }
}

async function loadGroups() {

    ClearPage();
    const table = document.getElementById('groupsTable');
    table.style = "display: flex;";

    const tbody = document.getElementById('groupBody');
    tbody.innerHTML = '';

    const res = await fetch('/groups/all');

    const groups = await res.json();

    for (const g of groups) {
        const tr = document.createElement('tr');
        tr.innerHTML = `
                    <td>${g.groupId}</td>
                    <td>${g.groupName}</td>`;
        tbody.appendChild(tr);
    }
}

async function addGroup() {
    ClearPage();
    let group = {
        groupName: document.getElementById("nameGroup").value
    }

    const res = await fetch('/groups/add', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
            groupName: group.groupName
        })
    });

}

async function addStudent() {
    ClearPage();
    let student = {
        firstName: document.getElementById("firstNameStudent").value,
        lastName: document.getElementById("lastNameStudent").value,
        age: Number(document.getElementById("ageStudent").value),
        groupId: Number(document.getElementById("groupId").value)
    }

    const res = await fetch('/students/add', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
            firstName: student.firstName,
            lastName: student.lastName,
            age: student.age,
            groupId: student.groupId
        })
    });

    number = await res.json();

    if (res.ok) Console.Log(`${number} SaveOk`);
    else Console.Log("NotOk");
}

async function updateStudent() {

    const id = Number(document.getElementById("idStudentUpdate").value)
    ClearPage();
    let student = {
        id: id,
        firstName: document.getElementById("firstNameUpdate").value,
        lastName: document.getElementById("lastNameUpdate").value,
        age: Number(document.getElementById("ageUpdate").value),
        groupId: Number(document.getElementById("groupIdUpdate").value)
    }

    const res = await fetch(`/students/${id}`, {
        method: 'PUT',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(student)
    });
}

async function deleteStudent() {
    const id = Number(document.getElementById("idStudentDelete").value)
    ClearPage();
    const res = await fetch(`/students/${id}`, {
        method: 'DELETE',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(id)
    });

}

async function loadTeachers() {

    ClearPage();
    const table = document.getElementById('teachersTable');
    table.style = "display: flex;";

    const tbody = document.getElementById('teacherBody');
    tbody.innerHTML = '';

    const res = await fetch('/teachers/all');
    if (!res.ok) return showResult(res);

    const teachers = await res.json();

    for (const t of teachers) {
        const tr = document.createElement('tr');
        tr.innerHTML = `
                    <td>${t.teacherId}</td>
                    <td>${t.firstName}</td>
                    <td>${t.lastName}</td>`;
        tbody.appendChild(tr);
    }
}

async function loadSubjects() {

    ClearPage();
    const table = document.getElementById('subjectsTable');
    table.style = "display: flex;";

    const tbody = document.getElementById('subjectBody');
    tbody.innerHTML = '';

    const res = await fetch('/subjects/all');
    if (!res.ok) return showResult(res);

    const subjects = await res.json();

    for (const s of subjects) {
        const tr = document.createElement('tr');
        tr.innerHTML = `
                    <td>${s.subjectsId}</td>
                    <td>${s.subjectsName}</td>
                    <td>${s.teacherName} (ID ${s.teacherId})</td>`;
        tbody.appendChild(tr);
    }
}

async function loadGrades() {

    ClearPage();
    const table = document.getElementById('gradesTable');
    table.style = "display: flex;";

    const tbody = document.getElementById('gradeBody');
    tbody.innerHTML = '';

    const res = await fetch('/grades/all');
    if (!res.ok) return showResult(res);

    const grades = await res.json();

    for (const g of grades) {
        const tr = document.createElement('tr');
        tr.innerHTML = `
                    <td>${g.gradesId}</td>
                    <td>${g.studentName}</td>
                    <td>${g.subjectsName}</td>
                    <td>${g.score}</td>
                    <td>${new Date(g.gradeDate).toLocaleDateString('ru-RU')}</td>`;
        tbody.appendChild(tr);
    }
}

async function addTeacher() {
    ClearPage();
    let teacher = {
        firstName: document.getElementById("firstNameTeacher").value,
        lastName: document.getElementById("lastNameTeacher").value
    }

    const res = await fetch('/teachers/add', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(teacher)
    });

    await showResult(res, "Учитель добавлен");
}

async function addSubject() {
    ClearPage();
    let subject = {
        subjectsName: document.getElementById("nameSubject").value,
        teacherId: Number(document.getElementById("teacherIdSubject").value)
    }

    const res = await fetch('/subjects/add', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(subject)
    });

    await showResult(res, "Предмет добавлен");
}

async function addGrade() {
    ClearPage();
    const date = document.getElementById("dateGrade").value;
    let grade = {
        studentId: Number(document.getElementById("studentIdGrade").value),
        subjectsId: Number(document.getElementById("subjectIdGrade").value),
        score: Number(document.getElementById("scoreGrade").value),
        gradeDate: date || new Date().toISOString().slice(0, 10)
    }

    const res = await fetch('/grades/add', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(grade)
    });

    await showResult(res, "Оценка поставлена");
}

async function showResult(res, okText) {
    const p = document.getElementById('gr');
    if (res.ok) p.textContent = okText;
    else p.textContent = `Ошибка: ${await res.text()}`;
}

function ClearPage() {
    const ts = document.getElementById('studentsTable');
    const tg = document.getElementById('groupsTable');
    ts.style = "display: none;"
    tg.style = "display: none;"
    document.getElementById('teachersTable').style = "display: none;"
    document.getElementById('subjectsTable').style = "display: none;"
    document.getElementById('gradesTable').style = "display: none;"
}