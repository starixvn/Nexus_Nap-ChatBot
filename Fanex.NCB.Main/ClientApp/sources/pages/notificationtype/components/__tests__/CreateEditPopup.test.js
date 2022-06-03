import React from 'react';
import { CreateEditPopup } from '../CreateEditPopup';
import { toast } from 'react-toastify';
import { shallow } from 'enzyme';
import { SelectField } from '../../../common/select-field/SelectField';
import { MultipleSelectField } from '../../../common/multiple-select-field/MultipleSelectField';
import { InputField } from '../../../common/input-field/InputField';

describe('CreateEditPopup', () => {

    afterAll(() => {
        jest.clearAllMocks();
    });

    beforeAll(() => {
        jest.useFakeTimers();
    });

    const initProps = {
        getSettings: jest.fn(),
        createEditPopupData: {
            editingItem: { 
                notificationTypeId: 1,
                notificationTypeName: { value: 'test', isValid: true },
                recipientTypeIds: { value: 'receipt', isValid: true },
                chatGroupTypeIds: { value: 'chat', isValid: true },
                systemId: { value: 123, isValid: true }
            }
        },
        settings: {
            recipientTypes: [{ id: 1, value: 'recipient' }],
            chatGroupTypes: [{ id: 2, value: 'chatGroup' }]
        }
    };

    describe('componentWillMount', () => {
        it('should call exactly props function', () => {
            // Arrange
            const props = {...initProps};

            // Act 
            const container = shallow(<CreateEditPopup {...props}/>);
            container.instance().componentWillMount();

            // Assert
            expect(props.getSettings).toHaveBeenCalled();
        });
    });

    describe('submitHandler', () => {
        it('should setState and call props function exactly when valid params', async () => {
            // Arrange
            const promise = Promise.resolve({}); 
            const props = {
                ...initProps,
                createEditNotificationType: jest.fn(() => promise)
            };
            const mockEvent = {
                preventDefault: jest.fn()
            };
            const state = {
                notificationTypeName: { isValid: true },
                recipientTypeIds: { isValid: true, value: '1,2' },
                chatGroupTypeIds: { isValid: true, value: '1,2' },
                systemId: { isValid: true }
            };

            // Act
            const container = shallow(<CreateEditPopup {...props}/>);
            container.instance().setState(state);
            await container.instance().submitHandler(mockEvent);

            // Assert
            expect(container.state().isSummited).toBe(true);
            expect(container.state().isLoading).toBe(true);
            expect(props.createEditNotificationType).toHaveBeenCalled();
        });

        it('should setState and call props function exactly when not valid param', async () => {
            // Arrange
            const promise = Promise.resolve({}); 
            const props = {
                ...initProps,
                createEditNotificationType: jest.fn(() => promise)
            };
            const mockEvent = {
                preventDefault: jest.fn()
            };
            const state = {
                notificationTypeName: { isValid: false },
                recipientTypeIds: { isValid: false, value: '1,2' },
                chatGroupTypeIds: { isValid: true, value: '1,2' },
                systemId: { isValid: true }
            };

            // Act
            const container = shallow(<CreateEditPopup {...props}/>);
            container.instance().setState(state);
            await container.instance().submitHandler(mockEvent);

            jest.runAllTimers();

            // Assert
            expect(container.state().isSummited).toBe(true);
            expect(container.state().isLoading).toBe(false);
        });
    });

    describe('getParams', () => {
        it('should return exactly data', () => {
            // Arrange
            const props = {...initProps};
            const state = {
                notificationTypeId: 1,
                notificationTypeName: { value: 'test' },
                recipientTypeIds: { value: '1,2' },
                chatGroupTypeIds: { value: '1,2' },
                systemId: { value: 1 }
            };

            // Act
            const container = shallow(<CreateEditPopup {...props}/>);
            container.setState(state);
            const actual = container.instance().getParams();

            // Assert
            expect(actual).toEqual({ chatGroupTypeIds: '1,2', notificationTypeId: 1, notificationTypeName: 'test', recipientTypeIds: '1,2', systemId: 1 });
        });
    });

    describe('checkValidParams', () => {
        it('should return true', () => {
            // Arrange
            const props = {...initProps};
            const state = {
                notificationTypeId: 1,
                notificationTypeName: { value: 'test', isValid: true },
                recipientTypeIds: { value: '1,2', isValid: true },
                chatGroupTypeIds: { value: '1,2', isValid: true },
                systemId: { value: 1, isValid: true }
            };

            // Act
            const container = shallow(<CreateEditPopup {...props}/>);
            container.setState(state);
            const actual = container.instance().checkValidParams();

            // Assert
            expect(actual).toBe(true);
        });

        it('should return false', () => {
            // Arrange
            const props = {...initProps};
            const state = {
                notificationTypeId: 1,
                notificationTypeName: { value: 'test', isValid: false },
                recipientTypeIds: { value: '1,2', isValid: true },
                chatGroupTypeIds: { value: '1,2', isValid: true },
                systemId: { value: 1, isValid: true }
            };

            // Act
            const container = shallow(<CreateEditPopup {...props}/>);
            container.setState(state);
            const actual = container.instance().checkValidParams();

            // Assert
            expect(actual).toBe(false);
        });
    });

    describe('onChange', () => {
        it('should setState exactly', () => {
            // Arrange
            const props = {...initProps};

            // Act
            const container = shallow(<CreateEditPopup {...props}/>);
            container.instance().onChange('notificationTypeId', 123);

            // Assert
            expect(container.state().notificationTypeId).toBe(123);
            expect(container.state().isChanged).toBe(true);
        });
    });

    describe('closePopup', () => {
        it('should call exactly function when successful', () => {
            // Arrange
            const props = {
                ...initProps,
                createEditResult: {
                    isSuccessful: true,
                    message: 'successfully'
                },
                createEditPopupData: {
                    editingItem: { 
                        notificationTypeId: 1,
                        notificationTypeName: { value: 'test', isValid: true },
                        recipientTypeIds: { value: 'receipt', isValid: true },
                        chatGroupTypeIds: { value: 'chat', isValid: true },
                        systemId: { value: 123, isValid: true }
                    },
                    successMsg: 'successfully!'
                },
                onClose: jest.fn()
            };

            toast.success = jest.fn();

            // Act
            const container = shallow(<CreateEditPopup {...props}/>);
            container.instance().closePopup();

            // Assert
            expect(toast.success).toHaveBeenCalledWith('successfully!');
            expect(props.onClose).toHaveBeenCalled();
        });

        it('should call exactly function when unSuccessful', () => {
            // Arrange
            const props = {
                ...initProps,
                createEditResult: {
                    isSuccessful: false,
                    message: 'false'
                },
                createEditPopupData: {
                    editingItem: { 
                        notificationTypeId: 1,
                        notificationTypeName: { value: 'test', isValid: true },
                        recipientTypeIds: { value: 'receipt', isValid: true },
                        chatGroupTypeIds: { value: 'chat', isValid: true },
                        systemId: { value: 123, isValid: true }
                    },
                    successMsg: 'false!'
                },
                onClose: jest.fn()
            };

            toast.error = jest.fn();

            // Act
            const container = shallow(<CreateEditPopup {...props}/>);
            container.instance().closePopup();

            jest.advanceTimersByTime(4000);

            // Assert
            expect(toast.error).toHaveBeenCalledWith('false');
            expect(setTimeout).toHaveBeenCalled();
            expect(container.state().isLoading).toBe(false);
        });
    });

    describe('closeConfirmation', () => {
        it('should setState exactly', () => {
            // Arrange
            const props = {...initProps};

            // Act
            const container = shallow(<CreateEditPopup {...props}/>);
            container.instance().closeConfirmation();

            // Assert
            expect(container.state().isShowedConfirm).toBe(false);
        });
    });

    describe('togglePopup', () => {
        it('should setState exactly when isChange true', () => {
            // Arrange
            const props = {...initProps};

            // Act
            const container = shallow(<CreateEditPopup {...props}/>);
            container.setState({ isChanged: true });
            container.instance().togglePopup();

            // Assert
            expect(container.state().isShowedConfirm).toBe(true);
        });

        it('should call exactly props function when isChange false', () => {
            // Arrange
            const props = {
                ...initProps,
                onClose: jest.fn()
            };

            // Act
            const container = shallow(<CreateEditPopup {...props}/>);
            container.setState({ isChanged: false });
            container.instance().togglePopup();

            // Assert
            expect(props.onClose).toHaveBeenCalled();
        });
    });

    describe('updateDropdownData', () => {
        it('should return exactly data', () => {
            // Arrange
            const props = {...initProps};
            const items = [{ id: 1, name: 'test' }, { id: 2, name: 'testing' }];
            const selectedIds = '1,2,3';

            // Act
            const container = shallow(<CreateEditPopup {...props}/>);
            const actual = container.instance().updateDropdownData(items, selectedIds);

            // Assert
            expect(actual).toEqual([{ checked: true, id: 1, name: 'test'}, { checked: true, id: 2, name: 'testing' }]);   
        });

        it('should return empty array', () => {
            // Arrange
            const props = {...initProps};
            const items = '';
            const selectedIds = '1,2,3';

            // Act
            const container = shallow(<CreateEditPopup {...props}/>);
            const actual = container.instance().updateDropdownData(items, selectedIds);

            // Assert
            expect(actual).toEqual([]);   
        });
    });

    describe('render', () => {
        it('should call exactly function InputField when change', () => {
            // Arrange
            const props = {...initProps};
            const onChange = jest.fn();

            // Act
            const container = shallow(<CreateEditPopup {...props}/>);
            container.instance().onChange = onChange;
            const render = shallow(container.instance().render());

            render.find(InputField).at(0).simulate('change', 123);

            // Assert
            expect(onChange).toHaveBeenCalledWith('notificationTypeName', 123);
            expect(render.find(InputField).at(0).length).toBe(1);
        });

        it('should call exactly function MultipleSelectField when change', () => {
            // Arrange
            const props = {...initProps};
            const onChange = jest.fn();

            // Act
            const container = shallow(<CreateEditPopup {...props}/>);
            container.instance().onChange = onChange;
            const render = shallow(container.instance().render());

            render.find(MultipleSelectField).at(0).simulate('change', 1);
            render.find(MultipleSelectField).at(1).simulate('change', 2);

            // Assert
            expect(onChange).toHaveBeenCalledWith('recipientTypeIds', 1);
            expect(onChange).toHaveBeenCalledWith('chatGroupTypeIds', 2);
            expect(render.find(MultipleSelectField).length).toBe(2);
        });

        it('should call exactly function SelectField when change', () => {
            // Arrange
            const props = {...initProps};
            const onChange = jest.fn();

            // Act
            const container = shallow(<CreateEditPopup {...props}/>);
            container.instance().onChange = onChange;
            const render = shallow(container.instance().render());

            render.find(SelectField).at(0).simulate('change', 1);

            // Assert
            expect(onChange).toHaveBeenCalledWith('systemId', 1);
            expect(render.find(SelectField).length).toBe(1);
        });
    });
});