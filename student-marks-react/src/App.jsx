import { useCallback, useEffect, useState } from 'react';
import './App.css';

const API_URL = 'https://studentmarks-api-dgfrfderbmcpbegk.southindia-01.azurewebsites.net/api/StudentMark';

export default function App() {
  const [students, setStudents] = useState([]);
  const [message, setMessage] = useState('');
  const [loading, setLoading] = useState(true);

  const [form, setForm] = useState({
    name: '',
    subject: '',
    marks: ''
  });

  const [editedMarks, setEditedMarks] = useState({});

  const loadStudents = useCallback(async () => {
    try {
      setLoading(true);

      const response = await fetch(API_URL);

      if (!response.ok) {
        throw new Error(`GET failed: ${response.status}`);
      }

      const data = await response.json();

      setStudents(data);

      const marksForEdit = {};
      data.forEach((student) => {
        marksForEdit[student.id] = student.marks;
      });

      setEditedMarks(marksForEdit);
    } catch (error) {
      setMessage(error.message);
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    loadStudents();
  }, [loadStudents]);

  function handleInputChange(event) {
    const { name, value } = event.target;

    setForm({
      ...form,
      [name]: value
    });
  }

  async function addStudent(event) {
    event.preventDefault();

    try {
      const response = await fetch(API_URL, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json'
        },
        body: JSON.stringify({
          id: 0,
          name: form.name,
          subject: form.subject,
          marks: Number(form.marks)
        })
      });

      if (!response.ok) {
        throw new Error(await response.text());
      }

      setMessage('Student mark added successfully.');
      setForm({ name: '', subject: '', marks: '' });
      await loadStudents();
    } catch (error) {
      setMessage(`Unable to add student: ${error.message}`);
    }
  }

  async function updateMarks(id) {
    const marks = Number(editedMarks[id]);

    if (marks < 0 || marks > 100) {
      setMessage('Marks must be between 0 and 100.');
      return;
    }

    try {
      const response = await fetch(
        `${API_URL}/${id}/marks?marks=${marks}`,
        {
          method: 'PUT'
        }
      );

      if (!response.ok) {
        throw new Error(await response.text());
      }

      setStudents((currentStudents) =>
        currentStudents.map((student) =>
          student.id === id ? { ...student, marks } : student
        )
      );

      setMessage('Marks updated successfully.');
    } catch (error) {
      setMessage(`Unable to update marks: ${error.message}`);
    }
  }

  async function deleteStudent(id) {
    if (!window.confirm('Delete this student record?')) {
      return;
    }

    try {
      const response = await fetch(`${API_URL}/${id}`, {
        method: 'DELETE'
      });

      if (!response.ok) {
        throw new Error(await response.text());
      }

      setStudents((currentStudents) =>
        currentStudents.filter((student) => student.id !== id)
      );

      setMessage('Student mark deleted successfully.');
    } catch (error) {
      setMessage(`Unable to delete student: ${error.message}`);
    }
  }

  return (
    <main className="container">
      <h1>Student Marks Management</h1>

      {message && <p className="message">{message}</p>}

      <section className="form-section">
        <h2>Add Student Mark</h2>

        <form onSubmit={addStudent}>
          <input
            name="name"
            value={form.name}
            onChange={handleInputChange}
            placeholder="Student name"
            maxLength="100"
            required
          />

          <input
            name="subject"
            value={form.subject}
            onChange={handleInputChange}
            placeholder="Subject"
            maxLength="100"
            required
          />

          <input
            name="marks"
            value={form.marks}
            onChange={handleInputChange}
            type="number"
            placeholder="Marks"
            min="0"
            max="100"
            required
          />

          <button type="submit">Add Student</button>
        </form>
      </section>

      <section>
        <h2>Student Marks</h2>

        {loading ? (
          <p>Loading students...</p>
        ) : (
          <table>
            <thead>
              <tr>
                <th>ID</th>
                <th>Name</th>
                <th>Subject</th>
                <th>Marks</th>
                <th>Update Marks</th>
                <th>Delete</th>
              </tr>
            </thead>

            <tbody>
              {students.map((student) => (
                <tr key={student.id}>
                  <td>{student.id}</td>
                  <td>{student.name}</td>
                  <td>{student.subject}</td>
                  <td>{student.marks}</td>
                  <td>
                    <input
                      className="marks-input"
                      type="number"
                      min="0"
                      max="100"
                      value={editedMarks[student.id] ?? ''}
                      onChange={(event) =>
                        setEditedMarks({
                          ...editedMarks,
                          [student.id]: event.target.value
                        })
                      }
                    />

                    <button onClick={() => updateMarks(student.id)}>
                      Update
                    </button>
                  </td>
                  <td>
                    <button
                      className="delete-button"
                      onClick={() => deleteStudent(student.id)}
                    >
                      Delete
                    </button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        )}
      </section>
    </main>
  );
}