import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { apiClient } from '../api/apiClient';
import { usePlayerService } from '../hooks/usePlayerService';
import Layout from '../components/Layout';
import styles from './ProfilePage.module.css';

const PROFILE_KEY = 'sudoku-profile';

export default function ProfilePage() {
  const navigate = useNavigate();
  const { playerAlias, isInitialized, isNewPlayer, clearPlayer } = usePlayerService();
  const [createdAt, setCreatedAt] = useState<string | null>(null);
  const [isEditing, setIsEditing] = useState(false);
  const [newAlias, setNewAlias] = useState('');
  const [editError, setEditError] = useState<string | null>(null);
  const [isSaving, setIsSaving] = useState(false);
  const [isLoading, setIsLoading] = useState(true);
  const [isConfirmingDelete, setIsConfirmingDelete] = useState(false);
  const [isDeleting, setIsDeleting] = useState(false);
  const [deleteError, setDeleteError] = useState<string | null>(null);

  useEffect(() => {
    if (isNewPlayer) navigate('/');
  }, [isNewPlayer, navigate]);

  useEffect(() => {
    if (!isInitialized || !playerAlias) return;

    const fetchProfile = async () => {
      setIsLoading(true);
      try {
        const result = await apiClient.getProfile(playerAlias);
        if (result.status === 200 && result.data) {
          setCreatedAt(result.data.createdAt);
        }
      } finally {
        setIsLoading(false);
      }
    };

    fetchProfile();
  }, [isInitialized, playerAlias]);

  const validateAlias = (value: string): string | null => {
    const trimmed = value.trim();
    if (trimmed.length < 2) return 'Alias must be at least 2 characters.';
    if (trimmed.length > 50) return 'Alias cannot exceed 50 characters.';
    if (!/^[a-zA-Z0-9 ]+$/.test(trimmed)) return 'Alias can only contain letters, numbers, and spaces.';
    return null;
  };

  const handleEditStart = () => {
    setNewAlias(playerAlias ?? '');
    setEditError(null);
    setIsEditing(true);
  };

  const handleEditCancel = () => {
    setIsEditing(false);
    setEditError(null);
  };

  const handleSaveAlias = async (e: React.FormEvent) => {
    e.preventDefault();
    setEditError(null);

    const validationError = validateAlias(newAlias);
    if (validationError) {
      setEditError(validationError);
      return;
    }

    if (!playerAlias) return;

    setIsSaving(true);
    try {
      const result = await apiClient.updateProfileAlias(playerAlias, newAlias.trim());

      if (result.status === 200 && result.data) {
        const stored = localStorage.getItem(PROFILE_KEY);
        if (stored) {
          const parsed = JSON.parse(stored);
          localStorage.setItem(PROFILE_KEY, JSON.stringify({ ...parsed, alias: result.data.alias }));
        }
        setIsEditing(false);
        navigate(0); // reload to reflect updated alias
        return;
      }

      if (result.status === 409) {
        setEditError('That alias is already taken. Please choose a different one.');
        return;
      }

      if (result.status === 404) {
        setEditError('Profile not found. Please refresh the page.');
        return;
      }

      setEditError('Something went wrong. Please try again.');
    } catch {
      setEditError('Failed to update alias. Please check your connection and try again.');
    } finally {
      setIsSaving(false);
    }
  };

  const handleDeleteProfile = async () => {
    if (!playerAlias) return;
    setIsDeleting(true);
    setDeleteError(null);
    try {
      const result = await apiClient.deleteProfile(playerAlias);
      if (result.status === 204) {
        clearPlayer();
        navigate('/');
        return;
      }
      if (result.status === 404) {
        setDeleteError('Profile not found. It may have already been deleted.');
        return;
      }
      setDeleteError('Something went wrong. Please try again.');
    } catch {
      setDeleteError('Failed to delete profile. Please check your connection and try again.');
    } finally {
      setIsDeleting(false);
    }
  };

  return (
    <Layout>
      <div className={styles.container}>
        <button className={styles.backButton} onClick={() => navigate('/')}>← Back</button>
        <h1 className={styles.title}>Your Profile</h1>

        {isLoading ? (
          <p>Loading…</p>
        ) : (
          <>
            <div className={styles.field}>
              <span className={styles.fieldLabel}>Alias</span>
              {isEditing ? (
                <form onSubmit={handleSaveAlias} className={styles.editForm}>
                  <input
                    type="text"
                    value={newAlias}
                    onChange={e => setNewAlias(e.target.value)}
                    className={styles.input}
                    maxLength={50}
                    aria-invalid={!!editError}
                    aria-describedby={editError ? 'alias-edit-error' : undefined}
                    autoFocus
                  />
                  {editError && (
                    <p id="alias-edit-error" className={styles.error} role="alert">{editError}</p>
                  )}
                  <div className={styles.editActions}>
                    <button
                      type="submit"
                      className={styles.saveButton}
                      disabled={isSaving || validateAlias(newAlias) !== null}
                    >
                      {isSaving ? 'Saving…' : 'Save'}
                    </button>
                    <button type="button" className={styles.cancelButton} onClick={handleEditCancel}>
                      Cancel
                    </button>
                  </div>
                </form>
              ) : (
                <div className={styles.fieldValue}>
                  <span>{playerAlias}</span>
                  <button className={styles.editButton} onClick={handleEditStart} aria-label="Edit alias">
                    Edit
                  </button>
                </div>
              )}
            </div>

            {createdAt && (
              <div className={styles.field}>
                <span className={styles.fieldLabel}>Member since</span>
                <span className={styles.fieldValue}>
                  {new Date(createdAt).toLocaleDateString()}
                </span>
              </div>
            )}

            <p className={styles.warning}>
              Your profile is stored in this browser. Clearing browser data will require you to create a new profile.
            </p>

            <div className={styles.deleteSection}>
              {isConfirmingDelete ? (
                <>
                  <p className={styles.deleteConfirmText}>
                    This will permanently delete your profile and all associated games. This cannot be undone.
                  </p>
                  {deleteError && (
                    <p className={styles.error} role="alert">{deleteError}</p>
                  )}
                  <div className={styles.deleteConfirmActions}>
                    <button
                      className={styles.deleteConfirmButton}
                      onClick={handleDeleteProfile}
                      disabled={isDeleting}
                    >
                      {isDeleting ? 'Deleting…' : 'Yes, delete my profile'}
                    </button>
                    <button
                      className={styles.cancelButton}
                      onClick={() => { setIsConfirmingDelete(false); setDeleteError(null); }}
                      disabled={isDeleting}
                    >
                      Cancel
                    </button>
                  </div>
                </>
              ) : (
                <button
                  className={styles.deleteButton}
                  onClick={() => setIsConfirmingDelete(true)}
                >
                  Delete Profile
                </button>
              )}
            </div>
          </>
        )}
      </div>
    </Layout>
  );
}
