import React from 'react';
import SignalR from '../SignalR';
import { shallow } from 'enzyme';

describe('SignalR', () => {
    afterAll(() => {
        jest.clearAllMocks();
    });

    describe('componentDidMount', () => {
        it('should call exactly function', () => {
            // Arrange
            const onUpdateNotificationStatus = jest.fn();

            // Act
            const container = shallow(<SignalR />);
            container.instance().connection = {
                on: jest.fn()
            };

            container.instance().onUpdateNotificationStatus = onUpdateNotificationStatus;
            container.instance().componentDidMount();

            // Assert
        });
    });

    describe('componentWillUnmount', () => {
        it('should call exactly function', () => {
            // Act
            const container = shallow(<SignalR />);
            container.instance().connection = {
                stop: jest.fn()
            };
            container.instance().componentWillUnmount();

            // Assert
            expect(container.instance().connection.stop).toHaveBeenCalled();
        });
    });

    describe('onInsertNotifications', () => {
        it('should call exactly props function', () => {
            // Arrange
            const props = {
                onInsertNewNotification: jest.fn()
            };

            // Act
            const container = shallow(<SignalR {...props}/>);
            container.instance().onInsertNotifications('123');

            // Assert
            expect(props.onInsertNewNotification).toHaveBeenCalledWith('123');
        });
    });

    describe('onUpdateNotificationStatus', () => {
        it('should call exactly props function', () => {
            // Arrange
            const props = {
                onUpdateNotificationStatus: jest.fn()
            };

            // Act
            const container = shallow(<SignalR {...props}/>);
            container.instance().onUpdateNotificationStatus('123');

            // Assert
            expect(props.onUpdateNotificationStatus).toHaveBeenCalledWith('123');
        });
    });
});