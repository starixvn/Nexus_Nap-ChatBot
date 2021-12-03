import React from 'react';
import { DataGrid } from '../DataGrid';
import { Row, Cell } from '../../../common/fixed-table-header/index';
import { Input } from 'reactstrap';
import { shallow } from 'enzyme';
import NoDataTitle from '../../../common/no-data-title/NoDataTitle';

describe('DataGrid', () => {
    beforeAll(() => {
        jest.useFakeTimers();
    });

    afterAll(() => {
        jest.clearAllMocks();
    });

    const initProps = {
        selected: [
            { id: 1, name: 'Special Delete', checked: true, newMaxMessageId: 0 },
            { id: 2, name: 'Special Delete - Internal CSD', checked: true, newMaxMessageId: 0 },
            { id: 3, name: 'Site Message', checked: true, newMaxMessageId: 0 }
        ],
        currentMessages: {
            data: [{ jobIdentify: 1, messageId: 1074533320 }, { jobIdentify: 2, messageId: 1074533325 }, { jobIdentify: 3, messageId: 1074533327 }],
            messageTypeInfos: [{ jobIdentify: 1, msgContentsType: 101 }]
        },
        newMessages: {
            newMaxMessageId: 1074533484,
            messagePostTimeInfos: [
                { messageId: 1074533320, postTime: "2021-10-11T03:31:53.687" },
                { messageId: 1074533325, postTime: "2021-10-11T03:31:53.687" },
                { messageId: 1074533327, postTime: "2021-10-11T03:31:53.687" }
            ]
        },
        missingMessageCsd: [{ messageId: 1074533326, requestId: 59630 }],
        missingMessages: [ 
            { messageId: 1074533321, jobIdentify: 1, content: 'Attn:[Soccer]', postTime: '2021-10-30T01:22:52.03' },
            { messageId: 1074533326, jobIdentify: 2, content: 'Attn:[Soccer]', postTime: '2021-10-30T01:22:52.03' },
            { messageId: 1074533328, jobIdentify: 3, content: 'Attn:[Soccer]', postTime: '2021-10-30T01:22:52.03' },
        ]
    };

    describe('componentDidMount', () => {
        it('should call exactly function getAllData', () => {
            // Arrange
            const props = { ...initProps };

            // Act
            const container = shallow(<DataGrid {...props}/>);
            container.instance().getAllData = jest.fn();
            container.instance().componentDidMount();

            // Assert
            expect(container.instance().getAllData).toHaveBeenCalled();
        });
    });

    describe('getAllData', () => {
        it('should call exactly props functions', async () => {
            // Arrange
            const props = {
                ...initProps,
                missingMessages: [ 
                    { messageId: 1074533321, jobIdentify: 1, content: 'Attn:[Soccer]', postTime: '2021-10-30T01:22:52.03' },
                    { messageId: 1074533326, jobIdentify: 2, content: 'Attn:[Soccer]', postTime: '2021-10-30T01:22:52.03' }
                ],
                getCurrentPreviousMaxMessage: jest.fn(),
                getNewPreviousMaxMessage: jest.fn(),
                getMissingMessageCsd: jest.fn(),
                getMissingMessageInfo: jest.fn(),
                setSelected: jest.fn()
            };

            // Act
            const container = shallow(<DataGrid {...props}/>);
            await container.instance().getAllData();

            // Assert
            expect(props.getCurrentPreviousMaxMessage).toHaveBeenCalled();
            expect(props.getNewPreviousMaxMessage).toHaveBeenCalled();
            expect(props.getMissingMessageCsd).toHaveBeenCalled();
            expect(props.getMissingMessageInfo).toHaveBeenCalled();
            expect(props.setSelected).toHaveBeenCalled();
        });
    });

    describe('isNotEqualCurrentNew', () => {
        it('should return true', () => {
            // Arrange
            const props = { ...initProps };

            // Act
            const container = shallow(<DataGrid {...props}/>);
            const actual = container.instance().isNotEqualCurrentNew(0);

            // Assert
            expect(actual).toBe(true);
        });

        it('should return false', () => {
            // Arrange
            const props = {
                ...initProps,
                selected: [
                    { id: 1, name: 'Special Delete', checked: true, newMaxMessageId: 1074533320 },
                    { id: 2, name: 'Special Delete - Internal CSD', checked: true, newMaxMessageId: 0 },
                    { id: 3, name: 'Site Message', checked: true, newMaxMessageId: 0 }
                ]
            };

            // Act
            const container = shallow(<DataGrid {...props}/>);
            const actual = container.instance().isNotEqualCurrentNew(0);

            // Assert
            expect(actual).toBe(false);
        });
    });

    describe('isAllEqualCurrentNew', () => {
        it('should return false', () => {
            // Arrange
            const props = { ...initProps };

            // Act
            const container = shallow(<DataGrid {...props}/>);
            const actual = container.instance().isAllEqualCurrentNew();

            // Assert
            expect(actual).toBe(false);
        });

        it('should return true', () => {
            // Arrange
            const props = {
                ...initProps,
                selected: [
                    { id: 1, name: 'Special Delete', checked: true, newMaxMessageId: 1074533320 },
                    { id: 2, name: 'Special Delete - Internal CSD', checked: true, newMaxMessageId: 1074533325 },
                    { id: 3, name: 'Site Message', checked: true, newMaxMessageId: 1074533327 }
                ]
            };

            // Act
            const container = shallow(<DataGrid {...props}/>);
            const actual = container.instance().isAllEqualCurrentNew();

            // Assert
            expect(actual).toBe(true);
        });
    })

    describe('handleShowEditNewMessage', () => {
        it('should call exactly props function', () => {
            // Arrange
            const props = {
                ...initProps,
                setEditNewMessagePopup: jest.fn()
            };

            // Act
            const container = shallow(<DataGrid {...props}/>);
            container.instance().handleShowEditNewMessage(1);

            // Assert
            expect(props.setEditNewMessagePopup).toHaveBeenCalledWith({
                show: true,
                jobIdentify: 1,
                title: 'Special Delete',
                newMaxMessageId: 0
            })
        });
    });

    describe('handleChange', () => {
        it('should setState correctly', () => {
            // Arrange
            const props = { ...initProps };
            const mockEvent = { target: { value: '0' } }

            // Act
            const container = shallow(<DataGrid {...props}/>);
            container.instance().handleChange(mockEvent);

            // Assert
            expect(container.state().checked).toEqual([true, false, false]);
            expect(container.state().isSubmitting).toEqual(false);
        });

        it('should setState correctly when not filter any item', () => {
            // Arrange
            const props = {
                ...initProps,
                selected: [
                    { id: 1, name: 'Special Delete', checked: false, newMaxMessageId: 1074533320 },
                    { id: 2, name: 'Special Delete - Internal CSD', checked: false, newMaxMessageId: 0 },
                    { id: 3, name: 'Site Message', checked: false, newMaxMessageId: 0 }
                ]
            };
            const mockEvent = { target: { value: '0' } }

            // Act
            const container = shallow(<DataGrid {...props}/>);
            container.instance().handleChange(mockEvent);

            // Assert
            expect(container.state().checked).toEqual([false, false, false]);
            expect(container.state().isSubmitting).toEqual(true);
        });
    });

    describe('handleAllChange', () => {
        it('should setState correctly', () => {
            // Arrange
            const props = { ...initProps };

            // Act
            const container = shallow(<DataGrid {...props}/>);
            container.instance().handleAllChange();

            // Assert
            expect(container.state().allChecked).toEqual(true);
            expect(container.state().checked).toEqual([true,true,true]);
            expect(container.state().isSubmitting).toEqual(false);
        });

        it('should return', () => {
            // Arrange
            const props = { ...initProps };

            // Act
            const container = shallow(<DataGrid {...props}/>);
            container.instance().isNotEqualCurrentNew = jest.fn(() => false);
            const actual = container.instance().handleAllChange();

            // Assert
            expect(actual).toBe(undefined);
        });
    });

    describe('checkCondition', () => {
        const props = {
            ...initProps,
            selected: [
                { id: 1, name: 'Special Delete', checked: true, newMaxMessageId: 1074533330 },
                { id: 2, name: 'Special Delete - Internal CSD', checked: true, newMaxMessageId: 0 },
                { id: 3, name: 'Site Message', checked: true, newMaxMessageId: 0 }
            ]
        };
        it('should return false', () => {
            // Arrange
            const item = { messageId: 1074533325 }

            // Act
            const container = shallow(<DataGrid {...props}/>);
            const actual = container.instance().checkCondition(item, 0);

            // Assert
            expect(actual).toBe(false);
        });

        it('should return false', () => {
            // Arrange
            const item = { messageId: 1074533315 }

            // Act
            const container = shallow(<DataGrid {...props}/>);
            const actual = container.instance().checkCondition(item, 0);

            // Assert
            expect(actual).toBe(false);
        })
    });

    describe('handleShowConfirm', () => {
        it('should setState correctly', () => {
            // Arrange
            const props = {...initProps};

            // Act
            const container = shallow(<DataGrid {...props}/>);
            container.instance().handleShowConfirm();

            // Assert
            expect(container.state().isShowConfirmPopup).toBe(true);
        });
    });

    describe('handleCloseConfirm', () => {
        it('should setState correctly', () => {
            // Arrange
            const props = {...initProps};

            // Act
            const container = shallow(<DataGrid {...props}/>);
            container.instance().handleCloseConfirm();

            // Assert
            expect(container.state().isShowConfirmPopup).toBe(false);
        });
    });

    describe('handleSubmit', () => {
        it('should call exactly props functions', async () => {
            // Arrange
            const promise = Promise.resolve({ data: { code: 0, isSuccessful: true }});
            const props = {
                ...initProps,
                selected: [
                    { id: 1, name: 'Special Delete', checked: true, newMaxMessageId: 1074533330 },
                    { id: 2, name: 'Special Delete - Internal CSD', checked: true, newMaxMessageId: 1074533330 },
                    { id: 3, name: 'Site Message', checked: true, newMaxMessageId: 1074533330 }
                ],
                createMissingMessage: jest.fn(() => promise),
                editMessageInfo: jest.fn(() => promise)
            };

            // Act
            const container = shallow(<DataGrid {...props}/>);
            container.instance().setState({ checked: [true, true, true] });
            container.instance().setSubmit = jest.fn();
            container.instance().handleSubmit();
            await promise;
            jest.runAllTimers();

            // Assert
            expect(props.createMissingMessage).toHaveBeenCalledWith({
                messages: [
                    {jobIdentify: 1, messageId: 1074533321},
                    {jobIdentify: 2, messageId: 1074533326},
                    {jobIdentify: 3, messageId: 1074533328}
                ],
                requests: [{ messageId: 1074533326, requestId: 59630 }]
            });
            expect(props.editMessageInfo).toHaveBeenCalledWith({
                jobInfoList: [
                    {jobIdentify: 1, messageId: 1074533330},
                    {jobIdentify: 2, messageId: 1074533330},
                    {jobIdentify: 3, messageId: 1074533330}
                ]
            });
            expect(await container.instance().setSubmit).toHaveBeenCalled();
            expect(setTimeout).toHaveBeenCalledTimes(1);
            expect(container.state().isSubmitting).toBe(true);
        });
    });

    describe('buildHeader', () => {
        it('should render expected header when iHost is true', () => {
            // Arrange
            const props = { ...initProps };
            
            // Act
            const container = shallow(<DataGrid {...props}/>).instance();
            container.handleAllChange = jest.fn();
            const header = shallow(container.buildHeader(true));
            header.find(Input).simulate('change');

            // Assert
            expect(header.find(Cell).length).toBe(4);
            expect(container.handleAllChange).toHaveBeenCalled();
        });

        it('should render expected header when iHost is false', () => {
            // Arrange
            const props = { ...initProps };
            
            // Act
            const container = shallow(<DataGrid {...props}/>).instance();
            const header = shallow(container.buildHeader(false));

            // Assert
            expect(header.find(Cell).length).toBe(3);
        });
    });

    describe('buildBody', () => {
        it('should render no data when data is empty and isHost is true', () => {
            // Arrange
            const props = {
                ...initProps,
                selected: [
                    { id: 1, name: 'Special Delete', checked: false, newMaxMessageId: 1074533330 },
                    { id: 2, name: 'Special Delete - Internal CSD', checked: false, newMaxMessageId: 1074533330 },
                    { id: 3, name: 'Site Message', checked: false, newMaxMessageId: 1074533330 }
                ]
            };

            // Act
            const container = shallow(<DataGrid {...props}/>);
            const actual = container.instance().buildBody(true);

            // Assert
            expect(actual).toEqual([<Row key={`body${1}`} ><Cell colSpan={4}><NoDataTitle /></Cell></Row >]);
        });

        it('should render no data when data is empty and isHost is false', () => {
            // Arrange
            const props = {
                ...initProps,
                selected: [
                    { id: 1, name: 'Special Delete', checked: false, newMaxMessageId: 1074533330 },
                    { id: 2, name: 'Special Delete - Internal CSD', checked: false, newMaxMessageId: 1074533330 },
                    { id: 3, name: 'Site Message', checked: false, newMaxMessageId: 1074533330 }
                ]
            };

            // Act
            const container = shallow(<DataGrid {...props}/>);
            const actual = container.instance().buildBody(false);

            // Assert
            expect(actual).toEqual([<Row key={`body${1}`} ><Cell colSpan={3}><NoDataTitle /></Cell></Row >]);
        });

        it('should render expected rows when data is not empty', () => {
            // Arrange
            const props = { ...initProps };

            // Act
            const container = shallow(<DataGrid {...props}/>).instance();
            container.renderRow = jest.fn(() => <div />);
            const actual = container.buildBody(true);

            // Assert
            expect(actual).toEqual([<div />,<div />,<div />]);
        });
    });

    describe('renderRow', () => {
        it('should call exactly function when change checkbox button', () => {
            // Arrange
            const props = {
                ...initProps
            };
            const item = {
                jobIdentify: 1
            };
            const mockEvent = { target: { value: 123 } };

            // Act
            const container = shallow(<DataGrid {...props} />);
            container.instance().handleChange = jest.fn();
            const renderRow = shallow(container.instance().renderRow(item, 1, false));
            renderRow.find(Input).simulate('change', mockEvent);

            // Assert
            expect(renderRow.find(Input).props().type).toBe('checkbox');
            expect(renderRow.find(Input).props().checked).toBe(false);
            expect(container.instance().handleChange).toHaveBeenCalledWith(mockEvent);
        });

        it('should call exactly function when click edit icon button', () => {
            // Arrange
            const props = {
                ...initProps
            };
            const item = {
                jobIdentify: 1
            };

            // Act
            const container = shallow(<DataGrid {...props} />);
            container.instance().handleShowEditNewMessage = jest.fn();
            const renderRow = shallow(container.instance().renderRow(item, 1, true));
            renderRow.find('.messageIcon').simulate('click');

            // Assert
            expect(container.instance().handleShowEditNewMessage).toHaveBeenCalledWith(1);
        });
    });

    describe('render', () => {
        it('should render null when props is null', () => {
            // Arrange
            const props = {
                ...initProps,
                currentMessages: null,
                newMessages: null,
                missingMessages: null
            };

            // Act
            const container = shallow(<DataGrid {...props} />);

            // Assert
            expect(container.type()).toEqual(null);
        });

        it('should call exactly function when click button submit', () => {
            // Arrange
            const props = { ...initProps };

            // Act
            const container = shallow(<DataGrid {...props} />);
            container.instance().handleShowConfirm = jest.fn();
            container.find('.btn-submit').simulate('click');

            // Assert
            expect(container.instance().handleShowConfirm).toHaveBeenCalled();
        });
    });
});