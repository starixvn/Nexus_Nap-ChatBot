import React from 'react';
import { EditNewMessage } from '../EditNewMessage';
import { Row, Cell } from '../../../common/fixed-table-header/index';
import { Input } from 'reactstrap';
import { shallow } from 'enzyme';
import NoDataTitle from '../../../common/no-data-title/NoDataTitle';
import EditNewMessageStore from '../EditNewMessage';
import configureMockStore from 'redux-mock-store';
import PanelContent from '../../../common/panel-content/PanelContent';
import ContentPreview from '../../../common/ContentPreview';

const mockStore = configureMockStore();

describe('EditNewMessage', () => {
    afterAll(() => {
        jest.clearAllMocks();
    });

    const initProps = {
        selected: [
            { id: 1, name: 'Special Delete', checked: true, newMaxMessageId: 0 },
            { id: 2, name: 'Special Delete - Internal CSD', checked: true, newMaxMessageId: 0 },
            { id: 3, name: 'Site Message', checked: true, newMaxMessageId: 0 }
        ],
        missingMessages: [ { messageId: 1, jobIdentify: 1, content: 'Attn:[Soccer]', postTime: '2021-10-30T01:22:52.03' } ]
    };

    describe('handleClose', () => {
        it('should call exactly prop function', () => {
            // Arrange
            const props = {
                ...initProps,
                setEditNewMessagePopup: jest.fn()
            };

            // Act
            const container = shallow(<EditNewMessage {...props}/>);
            container.instance().handleClose();

            // Assert
            expect(props.setEditNewMessagePopup).toHaveBeenCalledWith({ show: false });
        });
    });

    describe('toggleExpandPanels', () => {
        it('should set state when data lesser than 0', () => {
            // Arrange
            const props = {
                ...initProps,
                id: 0
            };

            // Act
            const container = shallow(<EditNewMessage {...props} />);
            container.instance().toggleExpandPanels();

            // Assert
            expect(container.state().expandPanels).toBe(true);
        });
    });

    describe('buildHeader', () => {
        it('should render expected header when expandPanels is true', () => {
            // Arrange
            const props = { ...initProps };
            const state = { expandPanels: true };
            
            // Act
            const container = shallow(<EditNewMessage {...props}/>).instance();
            container.setState(state);
            container.toggleExpandPanels = jest.fn();
            const header = shallow(container.buildHeader());
            header.find('span').at(0).simulate('click');

            // Assert
            expect(header.find(Cell).length).toBe(4);
            expect(header.find('span').at(0).props().className).toBe('icon-collapse');
            expect(container.toggleExpandPanels).toHaveBeenCalled();
        });

        it('should render expected header when expandPanels is false', () => {
            // Arrange
            const props = { ...initProps };
            const state = { expandPanels: false };
            
            // Act
            const container = shallow(<EditNewMessage {...props}/>).instance();
            container.setState(state);
            container.toggleExpandPanels = jest.fn();
            const header = shallow(container.buildHeader());
            header.find('span').at(0).simulate('click');

            // Assert
            expect(header.find(Cell).length).toBe(4);
            expect(header.find('span').at(0).props().className).toBe('icon-expand');
            expect(container.toggleExpandPanels).toHaveBeenCalled();
        });
    });

    describe('buildBody', () => {
        it('should render no data when data is empty', () => {
            // Arrange
            const props = { ...initProps };

            // Act
            const container = shallow(<EditNewMessage {...props}/>);
            const actual = container.instance().buildBody();

            // Assert
            expect(actual).toEqual([<Row key={`body${1}`} ><Cell colSpan="4"><NoDataTitle /></Cell></Row >]);
        });

        it('should render expected rows when data is not empty', () => {
            // Arrange
            const props = {
                ...initProps,
                id: 1
            };

            // Act
            const container = shallow(<EditNewMessage {...props}/>).instance();
            container.renderRow = jest.fn(() => <div />);
            const actual = container.buildBody();

            // Assert
            expect(actual).toEqual([<div />]);
        });
    });

    describe('renderRow', () => {
        it('should call exactly function when change radio button', () => {
            // Arrange
            const props = {
                ...initProps,
                newMaxId: 2
            };
            const item = {
                content: 'test',
                messageId: 2,
                postTime: '11/1/2021'
            };
            const mockEvent = { target: { value: 123 } };

            // Act
            const container = shallow(<EditNewMessage {...props} />);
            container.instance().handleChangeCheck = jest.fn();
            const renderRow = shallow(container.instance().renderRow(item,1));
            renderRow.find(Input).simulate('change', mockEvent);

            // Assert
            expect(renderRow.find(Input).props().type).toBe('radio');
            expect(renderRow.find(Input).props().defaultChecked).toBe(true);
            expect(container.instance().handleChangeCheck).toHaveBeenCalledWith(mockEvent);
        });

        it('PanelContent should be call', () => {
            // Arrange
            const props = {
                ...initProps,
                id: 1
            };
            const item = {
                content: `
                    test123456789123456789123456789123456789123456789123456798123
                    456789123456487976789123456789123456789123456789123456798123
                `
            };

            // Act
            const container = shallow(<EditNewMessage {...props} />);
            const renderRow = shallow(container.instance().renderRow(item,1));

            // Assert
            expect(renderRow.find(PanelContent).length).toBe(1);
            expect(renderRow.find(PanelContent).props().content).toBe(item.content);
        });

        it('ContentPreview should be call', () => {
            // Arrange
            const props = {
                ...initProps,
                id: 1
            };
            const item = {
                content: 'test'
            };

            // Act
            const container = shallow(<EditNewMessage {...props} />);
            const renderRow = shallow(container.instance().renderRow(item,1));

            // Assert
            expect(renderRow.find(ContentPreview).length).toBe(1);
            expect(renderRow.find(ContentPreview).props().text).toBe(item.content);
        });
    });

    describe('handleChangeCheck', () => {
        it('should set param selectedMessageId when change', () => {
            // Arrange
            const props = { ...initProps };
            const mockEvent = { target: { value: 123 } };

            // Act
            const container = shallow(<EditNewMessage {...props} />);
            container.instance().handleChangeCheck(mockEvent);

            // Assert
            expect(container.instance()['selectedMessageId']).toBe(123);
        });
    });

    describe('handleSubmit', () => {
        it('should call exactly functions', () => {
            // Arrange
            const props = {
                ...initProps,
                id: 1,
                setSelected: jest.fn()
            };
            const expected = [
                { id: 1, name: 'Special Delete', checked: true, newMaxMessageId: 123 },
                { id: 2, name: 'Special Delete - Internal CSD', checked: true, newMaxMessageId: 0 },
                { id: 3, name: 'Site Message', checked: true, newMaxMessageId: 0 }
            ];

            // Act
            const container = shallow(<EditNewMessage {...props} />).instance();
            container['selectedMessageId'] = 123;
            container.handleClose = jest.fn();
            container.handleSubmit();

            // Assert
            expect(props.setSelected).toHaveBeenCalledWith(expected);
            expect(container.handleClose).toHaveBeenCalled();
        });
    });

    describe('render', () => {
        it('should render null when missingMessages is null', () => {
            // Arrange
            const props = {
                ...initProps,
                missingMessages: null
            };

            // Act
            const container = shallow(<EditNewMessage {...props} />);

            // Assert
            expect(container.type()).toEqual(null);
        });

        it('should call exactly function onClose when onClick button', () => {
            // Arrange
            const props = { ...initProps };

            // Act
            const container = shallow(<EditNewMessage {...props} />);
            container.instance().handleClose = jest.fn();
            container.find('.btn-cancel').simulate('click');

            // Assert
            expect(container.instance().handleClose).toHaveBeenCalled();
        });

        it('should call exactly function handleSubmit when onClick button', () => {
            // Arrange
            const props = { ...initProps };

            // Act
            const container = shallow(<EditNewMessage {...props} />);
            container.instance().handleSubmit = jest.fn();
            container.find('.btn-submit').simulate('click');

            // Assert
            expect(container.instance().handleSubmit).toHaveBeenCalled();
        });

        it('props should be mapped correctly', () => {
            // Arrange
            const expected = {
                selected: [
                    { id: 1, name: 'Special Delete', checked: true, newMaxMessageId: 0 },
                    { id: 2, name: 'Special Delete - Internal CSD', checked: true, newMaxMessageId: 0 },
                    { id: 3, name: 'Site Message', checked: true, newMaxMessageId: 0 }
                ],
                missingMessages: [ { messageId: 1, jobIdentify: 1, content: 'Attn:[Soccer]', postTime: '2021-10-30T01:22:52.03' } ]
            };   
            const initialState = {
                selected: [
                    { id: 1, name: 'Special Delete', checked: true, newMaxMessageId: 0 },
                    { id: 2, name: 'Special Delete - Internal CSD', checked: true, newMaxMessageId: 0 },
                    { id: 3, name: 'Site Message', checked: true, newMaxMessageId: 0 }
                ],
                missingMessages: [ { messageId: 1, jobIdentify: 1, content: 'Attn:[Soccer]', postTime: '2021-10-30T01:22:52.03' } ]
            };

            const store = mockStore(initialState);

            // Act
            const container = shallow(<EditNewMessageStore store={store}/>);
            
            // Assert
            expect(container.props().selected).toEqual(expected.selected);       
            expect(container.props().missingMessages).toEqual(expected.missingMessages);       
        });
    });
});