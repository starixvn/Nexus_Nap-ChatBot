import React from 'react';
import { DataGrid } from '../DataGrid';
import { shallow } from 'enzyme';
import { Table, Row, Cell } from '../../../common/fixed-table-header/index';
import NoDataTitle from '../../../common/no-data-title/NoDataTitle';
import CommonFunctions from '../../../common/common-functions/CommonFunctions';

describe('DataGrid', () => {
    afterAll(() => {
        jest.clearAllMocks();
    });

    describe('onPaging', () => {
        it('should call correctly functions', () => {
            // Arrange
            const props = {
                gridParams: { pageSize: 100, pageIndex: 1 },
                setGridParams: jest.fn(),
                getGridData: jest.fn()
            };
            const expectedGridParams = { pageIndex: 1, pageSize: 10 };
            const container = shallow(<DataGrid {...props} />).instance();

            // Act
            container.onPaging(1, 10);

            // Assert
            expect(props.setGridParams).toHaveBeenCalledWith(expectedGridParams);
            expect(props.getGridData).toHaveBeenCalledWith(expectedGridParams);
        });
    });

    describe('openEditPopup', () => {
        it('bot is removed - do nothing', () => {
            // Arrange
            let item = {};
            let props = {
                filterRecipients: jest.fn(),
                setEditPopupData: jest.fn()
            };
            CommonFunctions.isBotRemoved = jest.fn(() => true);
            const container = shallow(<DataGrid {...props} />).instance();

            // Act
            container.openEditPopup(item);

            // Assert
            expect(props.filterRecipients).not.toHaveBeenCalled();
            expect(props.setEditPopupData).not.toHaveBeenCalled();
        });

        it('bot is not removed - should call correctly functions', () => {
            // Arrange
            let item = {
                groupID: 1, conversationID: 1, mappingStatusID: 2,
                mappingStatusName: '', botName: 'ncb', groupName: 'ncb',
                recipientTypeID: 12, recipientIDs: '1,2', recipientStatusNames: '',
                chatGroupTypeID: 1, remark: 'remark', channelId: 1, channelName: 'skype'
            };
            const expectedEditingItem = {
                groupID: 1, conversationID: 1,
                mappingStatusId: 2, mappingStatusName: '',
                botName: 'ncb', groupName: 'ncb',
                recipientTypeId: { value: 12, isValid: true },
                recipientIds: { value: '1,2', isValid: true },
                recipientStatusNames: '',
                chatGroupTypeId: { value: 1, isValid: true },
                remark: { value: 'remark', isValid: true },
                channelId: 1, channelName: 'skype'
            };
            let props = {
                filterRecipients: jest.fn(),
                setEditPopupData: jest.fn(),
                settings: {
                    recipients: []
                }
            };
            CommonFunctions.isBotRemoved = jest.fn(() => false);
            const container = shallow(<DataGrid {...props} />).instance();

            // Act
            container.openEditPopup(item);

            // Assert
            expect(props.filterRecipients).toHaveBeenCalledWith({ recipients: [], recipientTypeId: 12 });
            expect(props.setEditPopupData).toHaveBeenCalledWith({ isShowed: true, editingItem: expectedEditingItem });
        });
    });

    describe('getLicenseeStatus', () => {
        it('should return empty when recipientTypeName is invalid', () => {
            // Arrange
            let item = {
                recipientTypeName: null
            };
            const container = shallow(<DataGrid />).instance();

            // Act
            const actual = container.getLicenseeStatus(item);

            // Assert
            expect(actual).toEqual('');
        });

        it('should return correctly status when recipientTypeName is valid', () => {
            // Arrange
            let item = {
                recipientTypeName: 'Licensee',
                recipientStatusNames: 'Up Coming'
            };
            CommonFunctions.getLicenseeStatus = jest.fn(() => 'Up Coming');
            const container = shallow(<DataGrid />).instance();

            // Act
            const actual = container.getLicenseeStatus(item);

            // Assert
            expect(CommonFunctions.getLicenseeStatus).toHaveBeenCalledWith('Up Coming');
            expect(actual).toEqual('Up Coming');
        });
    });

    describe('getEditClassNames ', () => {
        it('bot is removed - return correctly className', () => {
            // Arrange
            const item = {};
            CommonFunctions.isBotRemoved = jest.fn(() => true);
            const container = shallow(<DataGrid />).instance();

            // Act
            const actual = container.getEditClassNames(item);

            // Assert
            expect(actual).toEqual('icon-edit edit-disabled');
        });

        it('bot is not removed - return correctly className', () => {
            // Arrange
            const item = {};
            CommonFunctions.isBotRemoved = jest.fn(() => false);
            const container = shallow(<DataGrid />).instance();

            // Act
            const actual = container.getEditClassNames(item);

            // Assert
            expect(actual).toEqual('icon-edit');
        });
    });

    describe('updateActive', () => {
        it('should call correctly functions when update is unsuccessful', async () => {
            // Arrange
            const item = { groupID: 1, isActive: true };
            const response = { data: { isSuccessful: false, message: 'unsuccessful' } };
            const promise = Promise.resolve(response);
            const props = {
                getGridData: jest.fn(),
                editActive: jest.fn(() => promise)
            };
            const container = shallow(<DataGrid {...props} />).instance();
            container.state = { item: item };
            container.toggleActive = jest.fn();

            // Act
            container.updateActive();
            await promise;

            // Assert
            expect(props.editActive).toHaveBeenCalledWith({ groupID: 1, isActive: false });
            expect(props.getGridData).not.toHaveBeenCalled();
            expect(container.toggleActive).toHaveBeenCalled();
        });

        it('should call correctly functions when update is successful', async () => {
            // Arrange
            const item = { groupID: 1, isActive: true };
            const response = { data: { isSuccessful: true, message: 'successful' } };
            const promise = Promise.resolve(response);
            const props = {
                gridParams: {},
                getGridData: jest.fn(),
                editActive: jest.fn(() => promise)
            };
            const container = shallow(<DataGrid {...props} />).instance();
            container.state = { item: item };
            container.toggleActive = jest.fn();

            // Act
            container.updateActive();
            await promise;

            // Assert
            expect(props.editActive).toHaveBeenCalledWith({ groupID: 1, isActive: false });
            expect(props.getGridData).toHaveBeenCalledWith({});
            expect(container.toggleActive).toHaveBeenCalled();
        });
    });

    describe('changeActive', () => {
        it('should update correctly state when isActive = true', () => {
            // Arrange
            const item = { isActive: true };
            const container = shallow(<DataGrid />);
            container.setState = jest.fn();

            // Act
            container.instance().changeActive(item);

            // Assert
            expect(container.setState).toHaveBeenCalledWith({
                isShowActivePopup: true,
                item: item,
                activeContent: 'When a group is inactivated. The system will not send notifications to the group. Do you want to proceed?'
            });
        });

        it('should update correctly state when isActive = false', () => {
            // Arrange
            const item = { isActive: false };
            const container = shallow(<DataGrid />);
            container.setState = jest.fn();

            // Act
            container.instance().changeActive(item);

            // Assert
            expect(container.setState).toHaveBeenCalledWith({
                isShowActivePopup: true,
                item: item,
                activeContent: 'When a group is activated. The system will send notifications to the group. Do you want to proceed?'
            });
        });
    });

    describe('toggleActive', () => {
        it('should update correctly state', () => {
            // Arrange
            const container = shallow(<DataGrid />);
            container.setState({ isShowActivePopup: false });

            // Act
            container.instance().toggleActive();

            // Assert
            expect(container.state().isShowActivePopup).toBe(true);
        });
    });

    describe('buildHeader', () => {
        const getHeader = function (isHost) {
            return (<Row style={{ height: 32 }}>
                <Cell colWidth={50} header>#</Cell>
                <Cell colWidth={100} header>Bot Name</Cell>
                <Cell colWidth={80} header>Bot Service</Cell>
                <Cell colWidth={60} header>Group ID</Cell>
                <Cell colWidth={200} header>Group Name</Cell>
                <Cell colWidth={110} header>Chat Group Type</Cell>
                <Cell colWidth={100} header>Recipient Type</Cell>
                <Cell colWidth={130} header>Recipient</Cell>
                <Cell colWidth={100} header>Licensee Status</Cell>
                <Cell colWidth={100} header>Mapping Status</Cell>
                <Cell colWidth={50} header>Active</Cell>
                <Cell colWidth={200} header>Remark</Cell>
                {
                    isHost &&
                    <Cell colWidth={60} header>Action</Cell>
                }
            </Row>);
        };

        it('should render expected header when isHost = false', () => {
            // Arrange
            const expected = getHeader(false);
            const container = shallow(<DataGrid />).instance();

            // Act
            const header = container.buildHeader(false);

            // Assert
            expect(header).toEqual(expected);
        });

        it('should render expected header when isHost = true', () => {
            // Arrange
            const expected = getHeader(true);
            const container = shallow(<DataGrid />).instance();

            // Act
            const header = container.buildHeader(true);

            // Assert
            expect(header).toEqual(expected);
        });
    });

    describe('buildBody', () => {
        it('should render no data when data is empty', () => {
            // Arrange
            const container = shallow(<DataGrid />).instance();

            // Act
            const actual = container.buildBody([], 1, 10, true);

            // Assert
            expect(actual).toEqual([<Row key={`body${1}`} ><Cell colSpan={13}><NoDataTitle /></Cell></Row >]);
        });

        it('should render expected rows when data is not empty', () => {
            // Arrange
            const data = [{ groupId: 1 }];
            const container = shallow(<DataGrid />).instance();
            container.renderRow = jest.fn(() => <div />);

            // Act
            const actual = container.buildBody(data, 1, 10, true);

            // Assert
            expect(actual).toEqual([<div />]);
        });
    });

    //    describe('render', () => {
    //        it('when gridData null - return null', () => {
    //            // Arrange
    //            let props = { gridData: null};
    //            let dataGrid = new DataGrid(props);

    //            // Act
    //            let result = dataGrid.render();

    //            // Assert
    //            expect(result).toBe(null);
    //        });

    //        it('when gridData.data null - return null', () => {
    //            // Arrange
    //            let props = {
    //                gridData: {data: null} };
    //            let dataGrid = new DataGrid(props);

    //            // Act
    //            let result = dataGrid.render();

    //            // Assert
    //            expect(result).toBe(null);
    //        });

    //        it('when gridData.data empty - render NoDataTitle', () => {
    //            // Arrange
    //            let props = {
    //                gridData: {
    //                    data: [],
    //                    count: 2
    //                },
    //                gridParams: {
    //                    pageIndex: 1,
    //                    pageSize: 10
    //                }
    //            };

    //            // Act
    //            let dataGrid = (new ShallowRenderer()).render(<DataGrid {...props} />);

    //            // Assert
    //            expect(dataGrid).toMatchSnapshot();
    //        });

    //        it('when has data - render table', () => {
    //            // Arrange
    //            let props = {
    //                gridData: {
    //                    data: [
    //                        {
    //                            botName: 'name 1',
    //                            conversationID: 1,
    //                            groupName: 'name 2',
    //                            recipientTypeName: 'name 3',
    //                            recipientNames: 'name 4',
    //                            recipientStatusNames: 'name 5',
    //                            mappingStatusID: 1,
    //                            mappingStatusName: 'name 6',
    //                            remark: 'name 7'
    //                        },
    //                        {
    //                            botName: 'name 11',
    //                            conversationID: 11,
    //                            groupName: 'name 21',
    //                            recipientTypeName: 'name 31',
    //                            recipientNames: 'name 41',
    //                            recipientStatusNames: 'name 51',
    //                            mappingStatusID: 11,
    //                            mappingStatusName: 'name 61',
    //                            remark: 'name 71'
    //                        }
    //                    ],
    //                    count: 2
    //                },
    //                gridParams: {
    //                    pageIndex: 1,
    //                    pageSize: 10
    //                }
    //            };

    //            // Act
    //            let dataGrid = (new ShallowRenderer()).render(<DataGrid {...props} />);

    //            // Assert
    //            expect(dataGrid).toMatchSnapshot();
    //        });
    //    });
});