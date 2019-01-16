import React from 'react';
import {NotificationType} from '../NotificationType';
import renderer from 'react-test-renderer';

describe('NotificationType', () => {
    describe('constructor', () => {
        it('init state', () => {
            const props = {}
            const notificationType = new NotificationType(props);

            expect(notificationType.state.isShowPopup).toBe(false);
            expect(notificationType.state.editingItem).toBe(null);
            expect(notificationType.state.isShowConfirm).toBe(false);
            expect(notificationType.state.activeRowIndex).toBe(-1);
            expect(notificationType.state.deletingNotificationTypeName).toBe('');
        })
    })
})