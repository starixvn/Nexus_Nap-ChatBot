import React from 'react';
import DataGrid from '../DataGrid';
import { shallow } from 'enzyme';
import { Row, Cell } from '../../../common/fixed-table-header/index';
import NoDataTitle from '../../../common/no-data-title/NoDataTitle';

describe('ErrorReport', () => {

    afterAll(() => {
        jest.clearAllMocks();
    });

    const initProps = {
        tableData: {
            pageIndex: 1,
            pageSize: 100,
            total: 100,
            data: [{ notificationId: 1, configSendTime: { toFormat: jest.fn() }, recipientSentStatusJson: [] }]
        }
    };

    describe('componentDidMount', () => {
        it('should call exactly function', () => {
            // Arrange
            const props = {...initProps};

            const notifications = document.createElement('div');
            notifications.setAttribute('class', 'hight-light-notification');
            notifications.scrollIntoView = jest.fn();
            document.body.appendChild(notifications);

            const container = shallow(<DataGrid {...props}/>);

            // Act
            container.instance().componentDidMount();
        });
    });

    describe('handleCancelNotification', () => {
        it('should call exactly props function', () => {
            // Arrange
            const props = {
                ...initProps,
                onCancelNotification: jest.fn()
            };
            const mockEvent = {
                target: {
                    dataset: { notificationid: 1 }
                }
            };

            const container = shallow(<DataGrid {...props}/>);

            // Act
            container.instance().handleCancelNotification(mockEvent);

            // Assert
            expect(props.onCancelNotification).toHaveBeenCalledWith(1);
        });
    });

    
    describe('toggleExpandPanels', () => {
        it('should setState exactly', () => {
            // Arrange
            const props = {...initProps};
            // Act
            const container = shallow(<DataGrid {...props} />);
            container.instance().toggleExpandPanels();

            // Assert
            expect(container.state().expandPanels).toBe(true);
        });
    });

    describe('handleShowErrorReport', () => {
        it('should call exactly props function', () => {
            // Arrange
            const props = {
                ...initProps,
                onShowErrorReport: jest.fn()
            };
            const mockEvent = {
                target: {
                    dataset: { notificationid: 1 }
                }
            };

            const container = shallow(<DataGrid {...props}/>);

            // Act
            container.instance().handleShowErrorReport(mockEvent);

            // Assert
            expect(props.onShowErrorReport).toHaveBeenCalledWith(1);
        });
    });

    describe('onPaging', () => {
        it('should call exactly props function', () => {
            // Arrange
            const props = {
                ...initProps,
                onPaging: jest.fn()
            };

            const container = shallow(<DataGrid {...props}/>);

            // Act
            container.instance().onPaging(1, 100);

            // Assert
            expect(props.onPaging).toHaveBeenCalledWith(1, 100);
        });
    });

    describe('getNotificationStatusClass', () => {
        it('should return exactly expected when statusId is 0', () => {
            // Arrange
            const props = {...initProps};

            const container = shallow(<DataGrid {...props}/>);

            // Act
            const actual = container.instance().getNotificationStatusClass(0);

            // Assert
            expect(actual).toEqual('badge-blue');
        });

        it('should return exactly expected when statusId is 1', () => {
            // Arrange
            const props = {...initProps};

            const container = shallow(<DataGrid {...props}/>);

            // Act
            const actual = container.instance().getNotificationStatusClass(1);

            // Assert
            expect(actual).toEqual('badge-green  ');
        });

        it('should return exactly expected when statusId is 2', () => {
            // Arrange
            const props = {...initProps};

            const container = shallow(<DataGrid {...props}/>);

            // Act
            const actual = container.instance().getNotificationStatusClass(2);

            // Assert
            expect(actual).toEqual('badge-red');
        });

        it('should return exactly expected when statusId is 3', () => {
            // Arrange
            const props = {...initProps};

            const container = shallow(<DataGrid {...props}/>);

            // Act
            const actual = container.instance().getNotificationStatusClass(3);

            // Assert
            expect(actual).toEqual('badge-outline-green');
        });

        it('should return exactly expected when statusId is 4', () => {
            // Arrange
            const props = {...initProps};

            const container = shallow(<DataGrid {...props}/>);

            // Act
            const actual = container.instance().getNotificationStatusClass(4);

            // Assert
            expect(actual).toEqual('badge-grey');
        });

        it('should return exactly expected when statusId is 5', () => {
            // Arrange
            const props = {...initProps};

            const container = shallow(<DataGrid {...props}/>);

            // Act
            const actual = container.instance().getNotificationStatusClass(5);

            // Assert
            expect(actual).toEqual('badge-orange');
        });

        it('should return exactly expected when statusId is 9', () => {
            // Arrange
            const props = {...initProps};

            const container = shallow(<DataGrid {...props}/>);

            // Act
            const actual = container.instance().getNotificationStatusClass(9);

            // Assert
            expect(actual).toEqual('badge-green-dark');
        });

        it('should return exactly expected when statusId is 10', () => {
            // Arrange
            const props = {...initProps};

            const container = shallow(<DataGrid {...props}/>);

            // Act
            const actual = container.instance().getNotificationStatusClass(10);

            // Assert
            expect(actual).toEqual('badge-yellow-dark');
        });

        it('should return exactly expected when statusId is 8', () => {
            // Arrange
            const props = {...initProps};

            const container = shallow(<DataGrid {...props}/>);

            // Act
            const actual = container.instance().getNotificationStatusClass(8);

            // Assert
            expect(actual).toEqual('');
        });
    });

    describe('buildHeader', () => {
        it('should render expected header when iHost is true', () => {
            // Arrange
            const props = { ...initProps };
            
            // Act
            const container = shallow(<DataGrid {...props}/>).instance();
            container.toggleExpandPanels = jest.fn();
            const header = shallow(container.buildHeader(true));
            header.find('span').simulate('click');

            // Assert
            expect(header.find(Cell).length).toBe(7);
            expect(container.toggleExpandPanels).toHaveBeenCalled();
        });

        it('should render expected header when iHost is false', () => {
            // Arrange
            const props = { ...initProps };
            
            // Act
            const container = shallow(<DataGrid {...props}/>).instance();
            const header = shallow(container.buildHeader(false));

            // Assert
            expect(header.find(Cell).length).toBe(6);
        });
    });

    describe('buildBody', () => {
        it('should render no data when data is empty and isHost is true', () => {
            // Arrange
            const props = {
                tableData: {
                    pageIndex: 1,
                    pageSize: 100,
                    total: 100,
                    data: []
                }
            }
            // Act
            const container = shallow(<DataGrid {...props}/>);
            const actual = container.instance().buildBody(true);

            // Assert
            expect(actual).toEqual([<Row key={`body${1}`} ><Cell colSpan="7"><NoDataTitle /></Cell></Row >]);
        });

        it('should render expected rows when data is not empty', () => {
            // Arrange
            const props = { ...initProps };

            // Act
            const container = shallow(<DataGrid {...props}/>).instance();
            container.renderRow = jest.fn(() => <div />);
            const actual = container.buildBody(true);

            // Assert
            expect(actual).toEqual([<div />]);
        });
    });

    describe('renderRecipient', () => {
        it('should render correctly', () => {
            // Arrange
            const props = { ...initProps };
            const recipient = {
                isError: 1,
                recipientName: 'test'
            };

            // Act
            const container = shallow(<DataGrid {...props}/>);
            const actual = container.instance().renderRecipient(recipient, 1);
        });
    });

    describe('renderNotificationId', () => {
        it('should render correctly', () => {
            // Arrange
            const props = { ...initProps };

            // Act
            const container = shallow(<DataGrid {...props}/>);
            const actual = container.instance().renderNotificationId(1, 1);
        });
    });

    describe('renderRow', () => {
        it('should render correctly', () => {
            // Arrange
            const props = {
                ...initProps,
                haveEditNotificationPermission: true
            };
            const data = { notificationId: 2, configSendTime: { toFormat: jest.fn() }, recipientSentStatusJson: [] };

            // Act
            const container = shallow(<DataGrid {...props} />);
            container.hightLightId = 1;
            const renderRow = shallow(container.instance().renderRow(data, 1, true));

            // Assert
            expect(renderRow.find(Cell).length).toBe(7);
        });
    });

    describe('renderAction', () => {
        it('should return exactly template is Error', () => {
            // Arrange
            const props = {
                ...initProps,
                haveEditNotificationPermission: true
            };
            const notification = {
                notificationId: 1,
                statusId: 2
            }

            // Act
            const container = shallow(<DataGrid {...props} />);
            const renderAction = container.instance().renderAction(notification);

            // Assert
            //expect(renderAction).toEqual();
        });

        it('should return exactly template is Resolve', () => {
            // Arrange
            const props = {
                ...initProps,
                haveEditNotificationPermission: true
            };
            const notification = {
                notificationId: 1,
                statusId: 5
            }

            // Act
            const container = shallow(<DataGrid {...props} />);
            const renderAction = container.instance().renderAction(notification);

            // Assert
            //expect(renderAction).toEqual();
        });

        it('should return exactly template is Planned', () => {
            // Arrange
            const props = {
                ...initProps,
                haveEditNotificationPermission: true
            };
            const notification = {
                notificationId: 1,
                statusId: 0
            }

            // Act
            const container = shallow(<DataGrid {...props} />);
            const renderAction = container.instance().renderAction(notification);

            // Assert
            expect(renderAction).toEqual(<a className="action-button" data-notificationid={1} onClick={expect.any(Function)} type="button">Cancel</a>);
        });

        it('should return exactly template is Cancelled', () => {
            // Arrange
            const props = {
                ...initProps,
                haveEditNotificationPermission: true
            };
            const notification = {
                notificationId: 1,
                statusId: 4
            }

            // Act
            const container = shallow(<DataGrid {...props} />);
            const renderAction = container.instance().renderAction(notification);

            // Assert
            expect(renderAction).toEqual(<a className="action-button" href="EditNotification/Index/1">Edit</a>);
        });

        it('should return exactly template is Sent', () => {
            // Arrange
            const props = {
                ...initProps,
                haveEditNotificationPermission: true
            };
            const notification = {
                notificationId: 1,
                statusId: 1
            }

            // Act
            const container = shallow(<DataGrid {...props} />);
            const renderAction = container.instance().renderAction(notification);

            // Assert
            expect(renderAction).toEqual(<a className="action-button" href="NewNotification?notificationId=1&recipientIds=&isResendAll=true" type="button">Resend</a>);
        });

        it('should return exactly template is null', () => {
            // Arrange
            const props = {
                ...initProps,
                haveEditNotificationPermission: true
            };
            const notification = {
                notificationId: 1,
                statusId: 123
            }

            // Act
            const container = shallow(<DataGrid {...props} />);
            const renderAction = container.instance().renderAction(notification);

            // Assert
            expect(renderAction).toEqual(null);
        });
    });

    describe('renderResendBtn', () => {
        it('should return exactly template', () => {
            // Arrange
            const props = {
                ...initProps,
                haveEditNotificationPermission: true
            };

            // Act
            const container = shallow(<DataGrid {...props} />);
            const renderResendBtn = container.instance().renderResendBtn(1, 'test', 2, true);

            // Assert
            expect(renderResendBtn).toEqual(<a className="action-button" href="NewNotification?notificationId=1&recipientIds=2&isResendAll=true" type="button">test</a>);
        });
    });
});