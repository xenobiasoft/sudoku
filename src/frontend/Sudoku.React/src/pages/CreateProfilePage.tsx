import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { apiClient } from '../api/apiClient';
import type { ProfileInfo } from '../types';
import styles from './CreateProfilePage.module.css';

const PROFILE_KEY = 'sudoku-profile';

export default function CreateProfilePage() {
  const navigate = useNavigate();
  const [alias, setAlias] = useState('');
  const [error, setError] = useState<string | null>(null);
  const [isSubmitting, setIsSubmitting] = useState(false);

  const validate = (value: string): string | null => {
    const trimmed = value.trim();
    if (trimmed.length < 2) return 'Alias must be at least 2 characters.';
    if (trimmed.length > 50) return 'Alias cannot exceed 50 characters.';
    if (!/^[a-z0-9 ]+$/i.test(trimmed)) return 'Alias can only contain letters, numbers, and spaces.';
    return null;
  };

  const handleSubmit = async (e: React.SubmitEvent<HTMLFormElement>) => {
    e.preventDefault();
    setError(null);

    const validationError = validate(alias);
    if (validationError) {
      setError(validationError);
      return;
    }

    setIsSubmitting(true);
    try {
      const result = await apiClient.createProfile(alias.trim());

      if (result.status === 201 && result.data) {
        const info: ProfileInfo = { profileId: result.data.profileId, alias: result.data.alias };
        localStorage.setItem(PROFILE_KEY, JSON.stringify(info));
        navigate('/');
        return;
      }

      if (result.status === 409) {
        setError('That alias is already taken. Please choose a different one.');
        return;
      }

      setError('Something went wrong. Please try again.');
    } catch {
      setError('Failed to create profile. Please check your connection and try again.');
    } finally {
      setIsSubmitting(false);
    }
  };

  const isValid = validate(alias) === null;

  return (
    <div className={styles.container}>
      <div className={styles.card}>
        <h1 className={styles.title}>Choose Your Alias</h1>
        <p className={styles.subtitle}>
          Pick a unique name to identify yourself. You can update it later from your profile page.
        </p>
        <form onSubmit={handleSubmit} className={styles.form}>
          <label htmlFor="alias" className={styles.label}>Alias</label>
          <input
            id="alias"
            type="text"
            value={alias}
            onChange={e => setAlias(e.target.value)}
            placeholder="e.g. sudoku master"
            className={styles.input}
            maxLength={50}
            aria-describedby={error ? 'alias-error' : undefined}
            aria-invalid={!!error}
            autoFocus
          />
          {error && (
            <p id="alias-error" className={styles.error} role="alert">{error}</p>
          )}
          <p className={styles.hint}>
            2–50 characters · letters, numbers, and spaces only · case-insensitive
          </p>
          <button
            type="submit"
            className={styles.button}
            disabled={!isValid || isSubmitting}
            aria-busy={isSubmitting}
          >
            {isSubmitting ? 'Creating…' : 'Create Profile'}
          </button>
        </form>
        <p className={styles.warning}>
          Your profile is stored in this browser. Clearing browser data will require you to create a new profile.
        </p>
      </div>
    </div>
  );
}
