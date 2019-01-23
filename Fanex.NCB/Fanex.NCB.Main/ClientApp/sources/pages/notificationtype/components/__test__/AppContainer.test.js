import React from 'react';
import {AppContainer} from '../AppContainer';
import renderer from 'react-test-renderer';
import ShallowRenderer from 'react-test-renderer/shallow';

describe('AppContainer', () => {
    describe('constructor', () => {
        it('init state', () => {
            const props = {};
            const notificationType = new AppContainer(props);

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
            const notificationType = renderer.create(<AppContainer {...props} />);
            const result = notificationType.toJSON();

            expect(result).toMatchSnapshot();
        })
        

        it('isShowPopup - false', () => {
            const props = {
                notificationTypeListData: {},
                getNotificationTypes: jest.fn()
            };
            const notificationType = renderer.create(<AppContainer {...props} />);
            const result = notificationType.toJSON();

            expect(result).toMatchSnapshot();
        })

        it('isShowPopup - true', () => {
            const props = {
                notificationTypeListData: {},
                getNotificationTypes: jest.fn(),
                isShowPopup: true
            };
            const notificationType = (new ShallowRenderer()).render(<AppContainer {...props} isShowPopup={true} />);            
            
            expect(notificationType).toMatchSnapshot();
        })
      
    })
})