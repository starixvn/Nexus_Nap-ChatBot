import React from 'react';
import {NotificationType} from '../NotificationType';
import renderer from 'react-test-renderer';
import ShallowRenderer from 'react-test-renderer/shallow';

describe('NotificationType', () => {
    describe('constructor', () => {
        it('init state', () => {
            const props = {};
            const notificationType = new NotificationType(props);

            expect(notificationType.state.isShowPopup).toBe(false);
            expect(notificationType.state.editingItem).toBe(null);
            expect(notificationType.state.isShowConfirm).toBe(false);
            expect(notificationType.state.activeRowIndex).toBe(-1);
            expect(notificationType.state.deletingNotificationTypeName).toBe('');
        })
    })

    describe('render', () => {
        it('notificationTypeListData - no data', () => {
            const props = {
                notificationTypeListData: {},
                getNotificationTypes: jest.fn()
            };
            const notificationType = renderer.create(<NotificationType {...props} />);
            const result = notificationType.toJSON();

            expect(result).toMatchSnapshot();
        })
        

        it('isShowPopup - false', () => {
            const props = {
                notificationTypeListData: {},
                getNotificationTypes: jest.fn()
            };
            const notificationType = renderer.create(<NotificationType {...props} />);
            const result = notificationType.toJSON();

            expect(result).toMatchSnapshot();
        })

        it('isShowPopup - true', () => {
            const props = {
                notificationTypeListData: {},
                getNotificationTypes: jest.fn(),
                isShowPopup: true
            };
            const notificationType = (new ShallowRenderer()).render(<NotificationType {...props} isShowPopup={true} />);            
            
            expect(notificationType).toMatchSnapshot();
        })
      
    })
})