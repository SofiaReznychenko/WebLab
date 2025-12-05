const API_BASE_URL = '/api';

document.addEventListener('DOMContentLoaded', () => {
    // Tab switching logic
    const tabs = document.querySelectorAll('.tab-button');
    tabs.forEach(tab => {
        tab.addEventListener('click', () => {
            document.querySelectorAll('.tab-button').forEach(t => t.classList.remove('active'));
            document.querySelectorAll('.tab-content').forEach(c => c.classList.remove('active'));

            tab.classList.add('active');
            const target = tab.dataset.target;
            document.getElementById(target).classList.add('active');

            if (target === 'members') loadMembers();
            if (target === 'workouts') loadWorkouts();
            if (target === 'trainers') loadTrainers();
        });
    });

    // Initial load
    loadMembers();

    // Form submissions
    document.getElementById('addMemberForm').addEventListener('submit', handleAddMember);
    document.getElementById('addWorkoutForm').addEventListener('submit', handleAddWorkout);
    document.getElementById('addTrainerForm').addEventListener('submit', handleAddTrainer);

    document.getElementById('editMemberForm').addEventListener('submit', handleEditMember);
    document.getElementById('editWorkoutForm').addEventListener('submit', handleEditWorkout);
    document.getElementById('editTrainerForm').addEventListener('submit', handleEditTrainer);

    // Close modals on outside click
    window.onclick = function (event) {
        if (event.target.classList.contains('modal')) {
            event.target.style.display = "none";
        }
    }
});

function closeModal(modalId) {
    document.getElementById(modalId).style.display = "none";
}

function handleError(error, errorElement) {
    try {
        const errorObj = JSON.parse(error.message);
        if (Array.isArray(errorObj)) {
            errorElement.textContent = errorObj.map(e => `${e.propertyName}: ${e.errorMessage}`).join(', ');
        } else {
            errorElement.textContent = error.message;
        }
    } catch {
        errorElement.textContent = error.message;
    }
}

// --- MEMBERS ---

async function loadMembers() {
    try {
        const response = await fetch(`${API_BASE_URL}/members`);
        const members = await response.json();

        const tbody = document.querySelector('#membersTable tbody');
        tbody.innerHTML = '';

        members.forEach(member => {
            const tr = document.createElement('tr');
            tr.innerHTML = `
                <td>${member.id}</td>
                <td>${member.name}</td>
                <td>${member.age}</td>
                <td>${member.gender}</td>
                <td>${member.email}</td>
                <td>${member.phone}</td>
                <td>${member.membershipType}</td>
                <td>${new Date(member.joinDate).toLocaleDateString()}</td>
                <td>${member.isActive ? 'Так' : 'Ні'}</td>
                <td>
                    <button class="btn-sm btn-edit" onclick="openEditMember(${member.id})">Ред.</button>
                    <button class="btn-sm btn-delete" onclick="deleteMember(${member.id})">Вид.</button>
                </td>
            `;
            tbody.appendChild(tr);
        });
    } catch (error) {
        console.error('Error loading members:', error);
    }
}

async function handleAddMember(e) {
    e.preventDefault();
    const form = e.target;
    const errorDiv = document.getElementById('memberError');
    errorDiv.textContent = '';

    const memberData = {
        name: form.name.value,
        email: form.email.value,
        phone: form.phone.value,
        membershipType: form.membershipType.value,
        age: parseInt(form.age.value),
        gender: form.gender.value
    };

    try {
        const response = await fetch(`${API_BASE_URL}/members`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(memberData)
        });

        if (!response.ok) {
            const error = await response.text();
            throw new Error(error);
        }

        form.reset();
        loadMembers();
    } catch (error) {
        handleError(error, errorDiv);
    }
}

async function deleteMember(id) {
    if (!confirm('Ви впевнені, що хочете видалити цього члена?')) return;

    try {
        const response = await fetch(`${API_BASE_URL}/members/${id}`, {
            method: 'DELETE'
        });
        if (response.ok) {
            loadMembers();
        } else {
            alert('Помилка видалення');
        }
    } catch (error) {
        console.error(error);
        alert('Помилка видалення');
    }
}

async function openEditMember(id) {
    try {
        const response = await fetch(`${API_BASE_URL}/members/${id}`);
        const member = await response.json();

        const form = document.getElementById('editMemberForm');
        form.id.value = member.id;
        form.name.value = member.name;
        form.email.value = member.email;
        form.phone.value = member.phone;
        form.membershipType.value = member.membershipType;
        form.age.value = member.age;
        form.gender.value = member.gender;
        form.isActive.checked = member.isActive;

        document.getElementById('editMemberModal').style.display = "block";
    } catch (error) {
        console.error(error);
        alert('Не вдалося завантажити дані');
    }
}

async function handleEditMember(e) {
    e.preventDefault();
    const form = e.target;
    const id = form.id.value;

    const memberData = {
        id: parseInt(id),
        name: form.name.value,
        email: form.email.value,
        phone: form.phone.value,
        membershipType: form.membershipType.value,
        age: parseInt(form.age.value),
        gender: form.gender.value,
        isActive: form.isActive.checked
    };

    try {
        const response = await fetch(`${API_BASE_URL}/members/${id}`, {
            method: 'PUT',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(memberData)
        });

        if (!response.ok) {
            const error = await response.text();
            throw new Error(error);
        }

        closeModal('editMemberModal');
        loadMembers();
    } catch (error) {
        alert('Помилка оновлення: ' + error.message);
    }
}

// --- WORKOUTS ---

async function loadWorkouts() {
    try {
        const response = await fetch(`${API_BASE_URL}/workouts`);
        const workouts = await response.json();

        const tbody = document.querySelector('#workoutsTable tbody');
        tbody.innerHTML = '';

        workouts.forEach(workout => {
            const tr = document.createElement('tr');
            tr.innerHTML = `
                <td>${workout.id}</td>
                <td>${workout.name}</td>
                <td>${workout.instructor}</td>
                <td>${workout.durationMinutes} хв</td>
                <td>${new Date(workout.scheduledDateTime).toLocaleString()}</td>
                <td>${workout.maxParticipants}</td>
                <td>
                    <button class="btn-sm btn-edit" onclick="openEditWorkout(${workout.id})">Ред.</button>
                    <button class="btn-sm btn-delete" onclick="deleteWorkout(${workout.id})">Вид.</button>
                </td>
            `;
            tbody.appendChild(tr);
        });
    } catch (error) {
        console.error('Error loading workouts:', error);
    }
}

async function handleAddWorkout(e) {
    e.preventDefault();
    const form = e.target;
    const errorDiv = document.getElementById('workoutError');
    errorDiv.textContent = '';

    const workoutData = {
        name: form.name.value,
        instructor: form.instructor.value,
        description: form.description.value,
        durationMinutes: parseInt(form.durationMinutes.value),
        maxParticipants: parseInt(form.maxParticipants.value),
        scheduledDateTime: form.scheduledDateTime.value,
        workoutType: form.workoutType.value
    };

    try {
        const response = await fetch(`${API_BASE_URL}/workouts`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(workoutData)
        });

        if (!response.ok) {
            const error = await response.text();
            throw new Error(error);
        }

        form.reset();
        loadWorkouts();
    } catch (error) {
        handleError(error, errorDiv);
    }
}

async function deleteWorkout(id) {
    if (!confirm('Ви впевнені, що хочете видалити це тренування?')) return;

    try {
        const response = await fetch(`${API_BASE_URL}/workouts/${id}`, {
            method: 'DELETE'
        });
        if (response.ok) {
            loadWorkouts();
        } else {
            alert('Помилка видалення');
        }
    } catch (error) {
        console.error(error);
        alert('Помилка видалення');
    }
}

async function openEditWorkout(id) {
    try {
        const response = await fetch(`${API_BASE_URL}/workouts/${id}`);
        const workout = await response.json();

        const form = document.getElementById('editWorkoutForm');
        form.id.value = workout.id;
        form.name.value = workout.name;
        form.instructor.value = workout.instructor;
        form.description.value = workout.description;
        form.durationMinutes.value = workout.durationMinutes;
        form.maxParticipants.value = workout.maxParticipants;
        form.scheduledDateTime.value = new Date(workout.scheduledDateTime).toISOString().slice(0, 16);
        form.workoutType.value = workout.workoutType;

        document.getElementById('editWorkoutModal').style.display = "block";
    } catch (error) {
        console.error(error);
        alert('Не вдалося завантажити дані');
    }
}

async function handleEditWorkout(e) {
    e.preventDefault();
    const form = e.target;
    const id = form.id.value;

    const workoutData = {
        id: parseInt(id),
        name: form.name.value,
        instructor: form.instructor.value,
        description: form.description.value,
        durationMinutes: parseInt(form.durationMinutes.value),
        maxParticipants: parseInt(form.maxParticipants.value),
        scheduledDateTime: form.scheduledDateTime.value,
        workoutType: form.workoutType.value
    };

    try {
        const response = await fetch(`${API_BASE_URL}/workouts/${id}`, {
            method: 'PUT',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(workoutData)
        });

        if (!response.ok) {
            const error = await response.text();
            throw new Error(error);
        }

        closeModal('editWorkoutModal');
        loadWorkouts();
    } catch (error) {
        alert('Помилка оновлення: ' + error.message);
    }
}

// --- TRAINERS ---

async function loadTrainers() {
    try {
        const response = await fetch(`${API_BASE_URL}/trainers`);
        const trainers = await response.json();

        const tbody = document.querySelector('#trainersTable tbody');
        tbody.innerHTML = '';

        trainers.forEach(trainer => {
            const tr = document.createElement('tr');
            tr.innerHTML = `
                <td>${trainer.id}</td>
                <td>${trainer.name}</td>
                <td>${trainer.age}</td>
                <td>${trainer.gender}</td>
                <td>${trainer.experience} р.</td>
                <td>${trainer.specialization || '-'}</td>
                <td>${trainer.phone}<br>${trainer.email}</td>
                <td>
                    <button class="btn-sm btn-edit" onclick="openEditTrainer(${trainer.id})">Ред.</button>
                    <button class="btn-sm btn-delete" onclick="deleteTrainer(${trainer.id})">Вид.</button>
                </td>
            `;
            tbody.appendChild(tr);
        });
    } catch (error) {
        console.error('Error loading trainers:', error);
    }
}

async function handleAddTrainer(e) {
    e.preventDefault();
    const form = e.target;
    const errorDiv = document.getElementById('trainerError');
    errorDiv.textContent = '';

    const trainerData = {
        name: form.name.value,
        age: parseInt(form.age.value),
        gender: form.gender.value,
        experience: parseInt(form.experience.value),
        specialization: form.specialization.value,
        phone: form.phone.value,
        email: form.email.value
    };

    try {
        const response = await fetch(`${API_BASE_URL}/trainers`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(trainerData)
        });

        if (!response.ok) {
            const error = await response.text();
            throw new Error(error);
        }

        form.reset();
        loadTrainers();
    } catch (error) {
        handleError(error, errorDiv);
    }
}

async function deleteTrainer(id) {
    if (!confirm('Ви впевнені, що хочете видалити цього тренера?')) return;

    try {
        const response = await fetch(`${API_BASE_URL}/trainers/${id}`, {
            method: 'DELETE'
        });
        if (response.ok) {
            loadTrainers();
        } else {
            alert('Помилка видалення');
        }
    } catch (error) {
        console.error(error);
        alert('Помилка видалення');
    }
}

async function openEditTrainer(id) {
    try {
        const response = await fetch(`${API_BASE_URL}/trainers/${id}`);
        const trainer = await response.json();

        const form = document.getElementById('editTrainerForm');
        form.id.value = trainer.id;
        form.name.value = trainer.name;
        form.age.value = trainer.age;
        form.gender.value = trainer.gender;
        form.experience.value = trainer.experience;
        form.specialization.value = trainer.specialization;
        form.phone.value = trainer.phone;
        form.email.value = trainer.email;

        document.getElementById('editTrainerModal').style.display = "block";
    } catch (error) {
        console.error(error);
        alert('Не вдалося завантажити дані');
    }
}

async function handleEditTrainer(e) {
    e.preventDefault();
    const form = e.target;
    const id = form.id.value;

    const trainerData = {
        id: parseInt(id),
        name: form.name.value,
        age: parseInt(form.age.value),
        gender: form.gender.value,
        experience: parseInt(form.experience.value),
        specialization: form.specialization.value,
        phone: form.phone.value,
        email: form.email.value
    };

    try {
        const response = await fetch(`${API_BASE_URL}/trainers/${id}`, {
            method: 'PUT',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(trainerData)
        });

        if (!response.ok) {
            const error = await response.text();
            throw new Error(error);
        }

        closeModal('editTrainerModal');
        loadTrainers();
    } catch (error) {
        alert('Помилка оновлення: ' + error.message);
    }
}
