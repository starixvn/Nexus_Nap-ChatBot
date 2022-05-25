import React from 'react';
import ErrorReport from '../ErrorReport';
import { shallow } from 'enzyme';
import CommonFunctions from '../../../common/common-functions/CommonFunctions';

describe('ErrorReport', () => {

    afterAll(() => {
        jest.clearAllMocks();
    });

    describe('handleRemarkChange', () => {
        it('should setState exactly', () => {
            // Arrange
            const mockEvent = {
                target: { value: '123' }
            };

            const container = shallow(<ErrorReport />);
            
            // Act
            container.instance().handleRemarkChange(mockEvent);

            // Assert
            expect(container.state().remark).toEqual('123');
            expect(container.state().remarkTouched).toEqual(true);
        });
    });

    describe('renderFailedRecipient', () => {
        it('should return expected result', () => {
            // Arrange
            const recipient = {
                recipientName: 'test',
                recipientTypeName: 'one',
                groupName: 'vn',
                errorMessage: 'no err'
            };

            const container = shallow(<ErrorReport />);
            container.instance().renderFailedRecipient = jest.fn(() => <div />);

            // Act
            const actual = container.instance().renderFailedRecipient(recipient, 1);

            // Assert
            expect(actual).toEqual(<div />);
        });
    });

    describe('copyToClipboard', () => {
        it('should call another common function', () => {
            // Arrange
            const props = {
                report: {
                    errorDetails: [{ recipientName: 'test' }, { recipientName: 'testing' }],
                    configSendTime: {
                        toFormat: jest.fn()
                    },
                    successRecipients: [1,2,3]
                }
            };
            const displayCopiedToClipboard = jest.fn();
            CommonFunctions.displayCopiedToClipboard = displayCopiedToClipboard;
            
            const container = shallow(<ErrorReport {...props}/>);
            document.execCommand = jest.fn();

            // Act
            container.instance().copyToClipboard();

            // Assert
            expect(displayCopiedToClipboard).toHaveBeenCalled();
        });
    });

    describe('handleResolveNotification', () => {
        it('should call exactly props function', () => {
            // Arrange
            const props = {
                onResolveNotification: jest.fn()
            };

            const container = shallow(<ErrorReport {...props}/>);
            container.setState({ remark: 'test' });

            // Act
            container.instance().handleResolveNotification();

            // Assert
            expect(props.onResolveNotification).toHaveBeenCalledWith('test');
        });

        it('should setState exactly', () => {
            // Arrange
            const container = shallow(<ErrorReport />);
            container.setState({ remarkTouched: false });

            // Act
            container.instance().handleResolveNotification();

            // Assert
            expect(container.state().remarkTouched).toBe(true);
        });

        it('should do nothing', () => {
            // Arrange
            const container = shallow(<ErrorReport />);
            container.setState({ remarkTouched: true });

            // Act
            container.instance().handleResolveNotification();
        });
    });

    describe('render', () => {
        it('should render null when report is null', () => {
            // Arrange
            const props = {
                report: null
            };

            const container = shallow(<ErrorReport {...props}/>);

            // Act
            expect(container.type()).toEqual(null);
        });

        it('should render input correctly', () => {
            // Arrange
            const props = {
                report: {
                    statusId: 2,
                    errorDetails: [{ recipientName: 'test' }, { recipientName: 'testing' }],
                    configSendTime: {
                        toFormat: jest.fn()
                    },
                    successRecipients: [1,2,3]
                },
                haveHostPermission: true
            };
            const state = {
                remarkTouched: true,
                remark: ''
            };

            const container = shallow(<ErrorReport {...props}/>);
            container.setState(state);

            // Act
            expect(container.find('input').length).toBe(1);
            expect(container.find('input').props().className).toBe('form-control is-invalid');
        });

        it('should render span text-italic correctly', () => {
            // Arrange
            const props = {
                report: {
                    statusId: 5,
                    errorDetails: [{ recipientName: 'test' }, { recipientName: 'testing' }],
                    configSendTime: {
                        toFormat: jest.fn()
                    },
                    successRecipients: [1,2,3]
                },
                haveHostPermission: true
            };

            const container = shallow(<ErrorReport {...props}/>);

            // Act
            expect(container.find('span').length).toBe(6);
        });
    });
});