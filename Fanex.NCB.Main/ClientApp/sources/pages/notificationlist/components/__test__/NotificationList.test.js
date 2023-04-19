import React from 'react';
import NotificationList from '../NotificationList';
import DataGrid from '../DataGrid';
import ErrorReport from '../ErrorReport';
import MultiselectTreeView from '../../../common/react-multiselect-treeview/MultiselectTreeView';
import DateRangePicker from 'react-bootstrap-daterangepicker';
import { Modal, ModalHeader, ModalBody, ModalFooter } from 'reactstrap';
import { Input } from 'reactstrap';
import { shallow } from 'enzyme';
import Site from '../../../lib/site';
import { DateTime } from 'luxon';
import axios from 'axios';

describe('NotificationList', () => {

    afterAll(() => {
        jest.clearAllMock();
    });

    beforeAll(() => {
        Site.resolveClientUrl = jest.fn(url => url);
        DateTime.fromISO = jest.fn(time => time);
        //JSON.parse = jest.fn(json => json);
        jest.useFakeTimers();
    });

    beforeEach(() => {
        const notificationPermission = document.createElement('div');
        const loadingProcess = document.createElement('div');
        notificationPermission.setAttribute('id', 'have-edit-notification-permission');
        notificationPermission.setAttribute('checked', true);
        loadingProcess.setAttribute('id', 'loadingProcess');
        document.body.appendChild(notificationPermission);
        document.body.appendChild(loadingProcess);
    });

    afterEach(() => {
        const notificationPermission = document.getElementById('have-edit-notification-permission');
        const loadingProcess = document.getElementById('loadingProcess');
        if (notificationPermission) {
            document.body.removeChild(notificationPermission);
        }
        if (loadingProcess) {
            document.body.removeChild(loadingProcess);
        }
    });

    describe('componentDidMount', () => {
        it('should setState exactly', async () => {
            // Arrange
            const getNotifications = jest.fn();

            const response = { data: [{ configSendTime: '2020/05/24', recipientSentStatusJson: {test: true}}], notificationTypes: [{id: 1, checked: true}] };
            const promise = Promise.resolve(response);
            axios.defaults = jest.fn(() => promise);

            // Act
            const container = shallow(<NotificationList />);
            container.instance().highlightNotification = { notificationId: 1 };
            container.instance().getNotifications = getNotifications();
            await container.instance().componentDidMount();

            // Assert
            promise.then(() => {
                expect(container.state().notificationTypeOptions).toEqual([]);
                expect(getNotifications).toHaveBeenCalled();
            });
        });
    });

    describe('handleOpenNotificationCancellation', () => {
        it('should setState exactly when call handleOpenNotificationCancellation', () => {
            // Arrange
            const expectedState = { show: true, notificationId: 1 };

            // Act
            const container = shallow(<NotificationList />);
            container.instance().handleOpenNotificationCancellation(1);

            // Assert
            expect(container.state().notificationCancellation).toEqual(expectedState);
        });
    });

    describe('handleCloseNotificationCancellation', () => {
        it('should setState exactly when call handleCloseNotificationCancellation', () => {
            // Arrange
            const expectedState = { show: false };

            // Act
            const container = shallow(<NotificationList />);
            container.instance().handleCloseNotificationCancellation();

            // Assert
            expect(container.state().notificationCancellation).toEqual(expectedState);
        });
    });

    describe('handleCancelNotification', () => {
        it('should call exactly function', async() => {
            // Arrange
            const state = {
                notificationCancellation: { notificationId: 1 }
            };
            const handleCloseNotificationCancellation = jest.fn();
            //const getNotifications = jest.fn();

            // Act
            const container = shallow(<NotificationList />);
            container.setState(state);
            container.instance().handleCloseNotificationCancellation = handleCloseNotificationCancellation;
            //container.instance().getNotifications = getNotifications;
            container.instance().handleCancelNotification();

            // Assert
            expect(handleCloseNotificationCancellation).toHaveBeenCalled();
            //expect(getNotifications).toHaveBeenCalled();
        });
    });

    describe('checkValidDateRange', () => {
        it('should return true', () => {
            const fromDate = new Date('2022-03-25');
            const toDate = new Date('2022-03-27');

            // Act
            const container = shallow(<NotificationList />);
            const actual = container.instance().checkValidDateRange(fromDate, toDate);

            // Assert
            expect(actual).toBe(true);
        });

        it('should return false', () => {
            // Arrange
            const fromDate = new Date('2022-03-25');
            const toDate = new Date('2022-04-27');

            // Act
            const container = shallow(<NotificationList />);
            const actual = container.instance().checkValidDateRange(fromDate, toDate);

            // Assert
            expect(actual).toBe(false);
        });
    });

    describe('handleCloseOutRangeDate', () => {
        it('should setState exactly', () => {
            // Act
            const container = shallow(<NotificationList />);
            container.instance().handleCloseOutRangeDate();

            // Assert
            expect(container.instance().state.outOfDateRange).toEqual({ show: false });
        });
    });

    describe('handleDateChange', () => {
        it('should setState exactly', () => {
            // Arrange
            const fromDate = { toDate: jest.fn(() => '2022-03-25')};
            const toDate = { toDate: jest.fn(() => '2022-03-27')};

            // Act
            const container = shallow(<NotificationList />);
            container.instance().handleDateChange(fromDate, toDate);

            // Assert
            expect(container.instance().state.filter.fromDate).toEqual('2022-03-25');
            expect(container.instance().state.filter.toDate).toEqual('2022-03-27');
        });
    });

    describe('getNotifications', () => {
        it('should setState exactly', async () => {
            // Arrange
            const state = {
                filter: {
                    messageId: 4294967297,
                    notificationTypeIds: 1,
                    fromDate: { toDateString: jest.fn(() => '2022/05/23'), getTime: jest.fn() },
                    toDate: { toDateString: jest.fn(() => '2022/05/25'), getTime: jest.fn() },
                    creatorId: 1,
                    recipientIds: 1,
                    statusIds: 1,
                    channelIds: 1
                }
            };
            const response = { data: { data: [{ configSendTime: '2020/05/24', recipientSentStatusJson: {test: true}}] } };
            const promise = Promise.resolve(response);
            axios.get = jest.fn(() => promise);
            
            jest.runAllTimers();

            // Act
            const container = shallow(<NotificationList />);
            container.setState(state);
            container.checkValidDateRange = jest.fn(() => true);
            await container.instance().getNotifications(1, 100);

            // Assert
            expect(axios.get).toHaveBeenCalled();
            expect(container.state().filter).toEqual(state.filter);
            //expect(container.state().tableData).toEqual({data: [{configSendTime: '2020/05/24', recipientSentStatusJson: {test: true}}], pageIndex: 1, pageSize: 100});
        });
    });

    describe('handleFilterChange', () => {
        it('should setState exactly', () => {
            // Arrange
            const state = {
                filter: {
                    messageId: 4294967297,
                    notificationTypeIds: 1
                }
            };
            const expected = {
                messageId: 4294967297,
                notificationTypeIds: 1,
                channelIds: 100
            };

            // Act
            const container = shallow(<NotificationList />);
            container.setState(state);
            container.instance().handleFilterChange('channelIds', 100);

            // Assert
            expect(container.state().filter).toEqual(expected);
        });
    });

    describe('handleFromDateChange', () => {
        it('should call exactly function', () => {
            // Arrange
            const value = '2022/05/25';
            const handleFilterChange = jest.fn();

            // Act
            const container = shallow(<NotificationList />);
            container.instance().handleFilterChange = handleFilterChange;
            container.instance().handleFromDateChange(value);

            // Assert
            expect(handleFilterChange).toHaveBeenCalledWith('fromDate', value);
        });
    });

    describe('handleToDateChange', () => {
        it('should call exactly function', () => {
            // Arrange
            const value = '2022/05/25';
            const handleFilterChange = jest.fn();

            // Act
            const container = shallow(<NotificationList />);
            container.instance().handleFilterChange = handleFilterChange;
            container.instance().handleToDateChange(value);

            // Assert
            expect(handleFilterChange).toHaveBeenCalledWith('toDate', value);
        });
    });

    describe('handleCreatorChange', () => {
        it('should call exactly function', () => {
            // Arrange
            const mockEvent = {
                target: {
                    value: 1
                }
            };
            const handleFilterChange = jest.fn();

            // Act
            const container = shallow(<NotificationList />);
            container.instance().handleFilterChange = handleFilterChange;
            container.instance().handleCreatorChange(mockEvent);

            // Assert
            expect(handleFilterChange).toHaveBeenCalledWith('creatorId', 1);
        });
    });

    describe('handleMessageIdChange', () => {
        it('should call exactly function when value is empty', () => {
            // Arrange
            const mockEvent = {
                target: {
                    value: ''
                }
            };
            const handleFilterChange = jest.fn();

            // Act
            const container = shallow(<NotificationList />);
            container.instance().handleFilterChange = handleFilterChange;
            container.instance().handleMessageIdChange(mockEvent);

            // Assert
            expect(handleFilterChange).toHaveBeenCalledWith('messageId', '');
        });

        it('should call exactly function when value is number', () => {
            // Arrange
            const mockEvent = {
                target: {
                    value: 1
                }
            };
            const handleFilterChange = jest.fn();

            // Act
            const container = shallow(<NotificationList />);
            container.instance().handleFilterChange = handleFilterChange;
            container.instance().handleMessageIdChange(mockEvent);

            // Assert
            expect(handleFilterChange).toHaveBeenCalledWith('messageId', 1);
        });
    });

    describe('renderStatus', () => {
        it('should return exactly template', () => {
            // Arrange
            const status = {
                id: 1,
                name: 'Open'
            };
            const expected = <option key={1} value={1}>Open</option>;

            // Act
            const container = shallow(<NotificationList />);
            const actual = container.instance().renderStatus(status);

            // Assert
            expect(actual).toEqual(expected);
        });
    });

    describe('handleSearch', () => {
        it('should call exactly function', () => {
            // Arrange
            const mockEvent = {
                preventDefault: jest.fn()
            };
            const getNotifications = jest.fn();

            // Act
            const container = shallow(<NotificationList />);
            container.instance().getNotifications = getNotifications;
            container.instance().handleSearch(mockEvent);

            // Assert
            expect(getNotifications).toHaveBeenCalled();
        });
    });

    describe('handleResolveNotification', () => {
        it('should call exactly function', async () => {
            // Arrange
            // Act
            const container = shallow(<NotificationList />);
            await container.instance().handleResolveNotification('test');
        });
    });

    describe('handleShowErrorReport', () => {
        it('should call exactly function', async () => {
            // Arrange
            const response = { data: { configSendTime: '2020/05/24', recipientSentStatusJson: {test: true} } };
            const promise = Promise.resolve(response);
            axios.get = jest.fn(() => promise);

            jest.runAllTimers();

            // Act
            const container = shallow(<NotificationList />);
            await container.instance().handleShowErrorReport(1);

            // Assert
            expect(axios.get).toHaveBeenCalledWith('NotificationList/GetNotificationError', {params: { notificationId: 1}});
            expect(container.state().errorReport).toEqual({notificationId: 1, report: {configSendTime: '2020/05/24', recipientSentStatusJson: {test: true}}, show: true});
        });
    });

    describe('handleCloseErrorReport', () => {
        it('should setState exactly', () => {
            // Act
            const container = shallow(<NotificationList />);
            container.instance().handleCloseErrorReport();

            // Assert
            expect(container.state().errorReport).toEqual({ show: false });
        });
    });

    describe('renderUserOption', () => {
        it('should return exactly template', () => {
            // Arrange
            const user = {
                id: 1,
                name: 'Test'
            };
            const expected = <option key={1} value={1}>Test</option>;

            // Act
            const container = shallow(<NotificationList />);
            const actual = container.instance().renderUserOption(user);

            // Assert
            expect(actual).toEqual(expected);
        });
    });

    describe('onUpdateNotificationStatus', () => {
        it('should call exactly function', async () => {
            // Arrange
            const notificationIds = '1,2,3';
            const state = {
                tableData: {
                    data: [{ notificationId: 1 }]
                }
            };
            const data = [{ notificationId: 3 }, { notificationId: 2 }];
            const getNotificationIdsToUpdate = jest.fn();
            const getUpdatedNotification = jest.fn();

            // Act
            const container = shallow(<NotificationList />);
            container.instance().getNotificationIdsToUpdate = getNotificationIdsToUpdate;
            container.instance().getUpdatedNotification = getUpdatedNotification;

            container.setState(state);
            await container.instance().onUpdateNotificationStatus(1);

            // Assert
            expect(getNotificationIdsToUpdate).toHaveBeenCalled();
            expect(getUpdatedNotification).toHaveBeenCalled();
        });
    });

    describe('onInsertedNewNotification', () => {
        it('should call exactly function', () => {
            // Arrange
            const state = {
                tableData: {
                    data: [{ notificationId: 1 }]
                }
            };
            const getNewNotificationIds = jest.fn();
            const getUpdatedNotification = jest.fn();
            
            // Act
            const container = shallow(<NotificationList />);
            container.setState(state);
            container.instance().getNewNotificationIds = getNewNotificationIds;
            container.instance().getUpdatedNotification = getUpdatedNotification;
            container.instance().onInsertedNewNotification(1);

            // Assert
            expect(getNewNotificationIds).toHaveBeenCalled();
            expect(getUpdatedNotification).toHaveBeenCalled();
        });
    });

    describe('compareWithCurrentFilter', () => {
        it('should return exactly', () => {
            // Arrange
            const state = {
                filter: {
                    messageId: 4294967297,
                    notificationTypeIds: '1,2',
                    fromDate: '2022/05/23',
                    toDate: '2022/05/25',
                    creatorId: 1,
                    recipientIds: 1,
                    statusIds: '1,2',
                    channelIds: '1,2'
                },
                floatRecipients: [{ isLeaf: true, text: 'test' }, { isLeaf: true, text: '123' }]
            };
            const notification = [
                {
                    messageID: 4294967297,
                    notificationTypeIds: 1,
                    fromDate: '2022/05/23',
                    toDate: '2022/05/25',
                    creatorId: 1,
                    recipientIds: 1,
                    statusId: 1,
                    channelId: 1,
                    recipientSentStatusJson: [{ recipientName: 'test' }]
                }
            ];

            // Act
            const container = shallow(<NotificationList />);
            container.instance().setState(state);
            const actual = container.instance().compareWithCurrentFilter(notification);

            // Assert
            expect(actual).toEqual([]);
        });
    });

    describe('getNewNotificationIds', () => {
        it('should return exactly', () => {
            // Arrange
            const notificationIds = '1,2,3,4,5';
            const notifications = [
                { notificationId: 1 },
                { notificationId: 7 },
            ];

            // Act
            const container = shallow(<NotificationList />);
            const actual = container.instance().getNewNotificationIds(notificationIds, notifications);

            // Assert
            expect(actual).toEqual('2,3,4,5');
        });
    });

    describe('getNotificationIdsToUpdate', () => {
        it('should return exactly', () => {
            // Arrange
            const notificationIds = '1,2,3,4,5';
            const notifications = [
                { notificationId: 1 },
                { notificationId: 7 },
            ];

            // Act
            const container = shallow(<NotificationList />);
            const actual = container.instance().getNotificationIdsToUpdate(notificationIds, notifications);

            // Assert
            expect(actual).toEqual('1');
        });
    });

    describe('getUpdatedNotification', () => {
        it('should call exactly function', async () => {
            // Arrange
            const response = { data: [{ configSendTime: '2020/05/24', recipientSentStatusJson: {test: true}}] };
            const promise = Promise.resolve(response);
            axios.get = jest.fn(() => promise);

            const notificationIds = '1,2,3,4,5';
            const callBack = jest.fn();
            jest.runAllTimers();

            // Act
            const container = shallow(<NotificationList />);
            await container.instance().getUpdatedNotification(notificationIds, callBack);

            // Assert
            expect(axios.get).toHaveBeenCalledWith('NotificationList/GetUpdatedNotification', {params: { notificationIds: '1,2,3,4,5'}});
        });
    });

    describe('render', () => {
        it('should render DataGrid', () => {
            // Arrange
            const state = {
                tableData: {
                    data: [{ notificationId: 1 }]
                }
            };

            // Act
            const container = shallow(<NotificationList />);
            container.setState(state);
            const render = shallow(container.instance().render());

            // Assert
            expect(render.find(DataGrid).length).toBe(1);
        });

        it('should render ErrorReport', () => {
            // Arrange
            const state = {
                errorReport: {
                    show: true
                }
            };

            // Act
            const container = shallow(<NotificationList />);
            container.setState(state);
            const render = shallow(container.instance().render());

            // Assert
            expect(render.find(ErrorReport).length).toBe(1);
        });

        it('should render exactly DateRangePicker', () => {
            // Arrange
            // Act
            const container = shallow(<NotificationList />);
            const render = shallow(container.instance().render());

            // Assert
            expect(render.find(DateRangePicker).length).toBe(1);
        });

        it('should render exactly MultiselectTreeView', () => {
            // Arrange
            // Act
            const container = shallow(<NotificationList />);
            const render = shallow(container.instance().render());

            // Assert
            expect(render.find(MultiselectTreeView).length).toBe(4);
        });

        it('should render exactly Modal', () => {
            // Arrange
            const handleCloseOutRangeDate = jest.fn();
            // Act
            const container = shallow(<NotificationList />);
            container.instance().setState({ outOfDateRange: { show: true }});
            container.instance().handleCloseOutRangeDate = handleCloseOutRangeDate;
            const render = shallow(container.instance().render());
            render.find(ModalFooter).at(0).find('button').at(0).simulate('click');


            // Assert
            expect(render.find(ModalFooter).length).toBe(1);
            expect(handleCloseOutRangeDate).toHaveBeenCalled();
        });

        it('should handle onChange notificationTypeIds exactly MultiselectTreeView', () => {
            // Arrange
            const handleFilterChange = jest.fn();
            const value = [
                { id: 1 }, { id: 2 }
            ];

            // Act
            const container = shallow(<NotificationList />);
            container.instance().handleFilterChange = handleFilterChange;
            const render = shallow(container.instance().render());
            render.find(MultiselectTreeView).at(0).simulate('change', value);

            // Assert
            expect(handleFilterChange).toHaveBeenCalledWith('notificationTypeIds', '1,2');
        });

        it('should handle onChange recipientIds exactly MultiselectTreeView', () => {
            // Arrange
            const handleFilterChange = jest.fn();
            const value = [
                { id: 1 }, { id: 2 }
            ];

            // Act
            const container = shallow(<NotificationList />);
            container.instance().handleFilterChange = handleFilterChange;
            const render = shallow(container.instance().render());
            render.find(MultiselectTreeView).at(1).simulate('change', value);

            // Assert
            expect(handleFilterChange).toHaveBeenCalledWith('recipientIds', '1,2');
        });

        it('should handle onChange statusIds exactly MultiselectTreeView', () => {
            // Arrange
            const handleFilterChange = jest.fn();
            const value = [
                { id: 1 }, { id: 2 }
            ];

            // Act
            const container = shallow(<NotificationList />);
            container.instance().handleFilterChange = handleFilterChange;
            const render = shallow(container.instance().render());
            render.find(MultiselectTreeView).at(2).simulate('change', value);

            // Assert
            expect(handleFilterChange).toHaveBeenCalledWith('statusIds', '1,2');
        });

        it('should handle onChange channelIds exactly MultiselectTreeView', () => {
            // Arrange
            const handleFilterChange = jest.fn();
            const value = [
                { id: 1 }, { id: 2 }
            ];

            // Act
            const container = shallow(<NotificationList />);
            container.instance().handleFilterChange = handleFilterChange;
            const render = shallow(container.instance().render());
            render.find(MultiselectTreeView).at(3).simulate('change', value);

            // Assert
            expect(handleFilterChange).toHaveBeenCalledWith('channelIds', '1,2');
        });

        it('should handle onCallback exactly DateRangePicker', () => {
            // Arrange
            const handleDateChange = jest.fn();
            // Act
            const container = shallow(<NotificationList />);
            container.instance().handleDateChange = handleDateChange;
            const render = shallow(container.instance().render());
            render.find(DateRangePicker).at(0).simulate('callback');

            // Assert
            expect(handleDateChange).toHaveBeenCalled();
        });

        it('should render exactly Input when have creatorId', () => {
            // Arrange
            const state = {
                filter: {
                    creatorId: 1
                }
            };

            // Act
            const container = shallow(<NotificationList />);
            container.setState(state);
            const render = shallow(container.instance().render());

            // Assert
            expect(render.find(Input).at(0).props().value).toEqual(1);
        });

        it('should render exactly Input when not have creatorId', () => {
            // Arrange
            const state = {
                filter: {
                    creatorId: null
                }
            };

            // Act
            const container = shallow(<NotificationList />);
            container.setState(state);
            const render = shallow(container.instance().render());

            // Assert
            expect(render.find(Input).at(0).props().value).toEqual('');
        });
    });
});