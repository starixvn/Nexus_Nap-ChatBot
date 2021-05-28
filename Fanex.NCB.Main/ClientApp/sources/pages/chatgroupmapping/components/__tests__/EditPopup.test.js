import React from 'react';
import { EditPopup } from '../EditPopup';
import { shallow } from 'enzyme';
import CommonFunctions from '../../../common/common-functions/CommonFunctions';

describe('EditPopup', () => {
    afterAll(() => {
        jest.clearAllMocks();
    });

    describe('submitHandler', () => {
        const props = {
            editPopupData: {
                isShowed: false,
                editingItem: {
                    groupID: 1, conversationID: 1, recipientTypeId: 1,
                    chatGroupTypeId: 1, mappingStatusId: 1, recipientIds: '', remark: ''
                }
            }
        };

        it('should call correctly functions when params are invalid', () => {
            // Arrange
            const event = { preventDefault: jest.fn() };
            CommonFunctions.isMapped = jest.fn();
            const container = shallow(<EditPopup />).instance();
            container.setSubmittedState = jest.fn();
            container.checkValidParams = jest.fn(() => false);
            container.submitData = jest.fn();

            // Act
            container.submitHandler(event);

            // Assert
            expect(event.preventDefault).toHaveBeenCalled();
            expect(container.setSubmittedState).toHaveBeenCalled();
            expect(CommonFunctions.isMapped).not.toHaveBeenCalled();
            expect(container.submitData).not.toHaveBeenCalled();
        });

        it('should update correctly state when mappingStatusId = mapped', () => {
            // Arrange
            const event = { preventDefault: jest.fn() };
            CommonFunctions.isMapped = jest.fn(() => true);
            const container = shallow(<EditPopup {...props} />);
            container.setState = jest.fn();
            const instance = container.instance();
            instance.setSubmittedState = jest.fn();
            instance.checkValidParams = jest.fn(() => true);
            instance.submitData = jest.fn();

            // Act
            instance.submitHandler(event);

            // Assert
            expect(event.preventDefault).toHaveBeenCalled();
            expect(instance.setSubmittedState).toHaveBeenCalled();
            expect(CommonFunctions.isMapped).toHaveBeenCalledWith(1);
            expect(instance.submitData).not.toHaveBeenCalled();
            expect(container.setState).toHaveBeenCalledWith({
                isShowedConfirm: true,
                confirmTitle: 'Confirm Message',
                confirmContent: 'Change setting will impact sending notification to a group. Are you sure to proceed?',
                confirmType: 2
            });
        });

        it('should call correctly functions when mappingStatusId != mapped', () => {
            // Arrange
            const event = { preventDefault: jest.fn() };
            CommonFunctions.isMapped = jest.fn(() => false);
            const container = shallow(<EditPopup {...props} />);
            container.setState = jest.fn();
            const instance = container.instance();
            instance.setSubmittedState = jest.fn();
            instance.checkValidParams = jest.fn(() => true);
            instance.submitData = jest.fn();

            // Act
            instance.submitHandler(event);

            // Assert
            expect(event.preventDefault).toHaveBeenCalled();
            expect(instance.setSubmittedState).toHaveBeenCalled();
            expect(CommonFunctions.isMapped).toHaveBeenCalledWith(1);
            expect(instance.submitData).toHaveBeenCalled();
            expect(container.setState).not.toHaveBeenCalled();
        });
    });

    describe('submitData', () => {
        const editPopupData = {
            isShowed: false,
            editingItem: {
                groupID: 1, conversationID: 1, recipientTypeId: 1,
                chatGroupTypeId: 1, mappingStatusId: 1, recipientIds: '', remark: ''
            }
        };

        it('should close popup when edit successfully', async () => {
            // Arrange
            const promise = Promise.resolve({});
            const props = {
                editPopupData,
                editMapping: jest.fn(() => promise),
                editResult: { isSuccessful: true }
            };
            const expectedParam = { groupId: 1 };
            const container = shallow(<EditPopup {...props} />).instance();
            container.getParams = jest.fn(() => expectedParam);
            container.closePopupOnSuccess = jest.fn();

            // Act
            container.submitData();
            await promise;

            // Assert
            expect(props.editMapping).toHaveBeenCalledWith(expectedParam);
            expect(container.closePopupOnSuccess).toHaveBeenCalled();
        });

        it('do nothing when edit unsuccessfully', async () => {
            // Arrange
            const promise = Promise.resolve({});
            const props = {
                editPopupData,
                editMapping: jest.fn(() => promise),
                editResult: { isSuccessful: false }
            };
            const expectedParam = { groupId: 1 };
            const container = shallow(<EditPopup {...props} />).instance();
            container.getParams = jest.fn(() => expectedParam);
            container.closePopupOnSuccess = jest.fn();

            // Act
            container.submitData();
            await promise;

            // Assert
            expect(props.editMapping).toHaveBeenCalledWith(expectedParam);
            expect(container.closePopupOnSuccess).not.toHaveBeenCalled();
        });
    });

    //    describe('getParams', () => {
    //        it('get params from state', () => {
    //            // Arrange
    //            let props = {
    //                ...defaultProps
    //            };
    //            let updatePopup = new EditPopup(props);
    //            updatePopup.state = {
    //                groupID: 1,
    //                conversationID: 2,
    //                recipientTypeId: {
    //                    value: 3
    //                },
    //                chatGroupTypeId: {
    //                    value: 4
    //                },
    //                recipientIds: { value: '4,5' },
    //                remark: { value: 'remark' }
    //            };
    //            let expectedParams = {
    //                groupID: 1,
    //                conversationId: 2,
    //                recipientTypeId: 3,
    //                chatGroupTypeId: 4,
    //                recipientIds: '4,5',
    //                remark: 'remark'
    //            };

    //            // Act
    //            let result = updatePopup.getParams();

    //            // Assert
    //            expect(result).toEqual(expectedParams);
    //        });
    //    });

    //    describe('checkValidParams', () => {
    //        it('when all valid - return true', () => {
    //            // Arrange
    //            let props = {
    //                ...defaultProps
    //            };
    //            let updatePopup = new EditPopup(props);
    //            updatePopup.state = {
    //                recipientTypeId: { isValid: true },
    //                recipientIds: { isValid: true },
    //                chatGroupTypeId: { isValid: true }
    //            };

    //            // Act
    //            let result = updatePopup.checkValidParams();

    //            // Assert
    //            expect(result).toBe(true);
    //        });

    //        it('when recipientTypeId invalid - return false', () => {
    //            // Arrange
    //            let props = {
    //                ...defaultProps
    //            };
    //            let updatePopup = new EditPopup(props);
    //            updatePopup.state = {
    //                recipientTypeId: { isValid: false },
    //                recipientIds: { isValid: true },
    //                remark: { isValid: true },
    //                chatGroupTypeId: { isValid: true }
    //            };

    //            // Act
    //            let result = updatePopup.checkValidParams();

    //            // Assert
    //            expect(result).toBe(false);
    //        });

    //        it('when recipientIds invalid - return false', () => {
    //            // Arrange
    //            let props = {
    //                ...defaultProps
    //            };
    //            let updatePopup = new EditPopup(props);
    //            updatePopup.state = {
    //                recipientTypeId: { isValid: true },
    //                recipientIds: { isValid: false },
    //                remark: { isValid: true },
    //                chatGroupTypeId: { isValid: true }
    //            };

    //            // Act
    //            let result = updatePopup.checkValidParams();

    //            // Assert
    //            expect(result).toBe(false);
    //        });

    //        it('when chatGroupTypeId invalid - return false', () => {
    //            // Arrange
    //            let props = {
    //                ...defaultProps
    //            };
    //            let updatePopup = new EditPopup(props);
    //            updatePopup.state = {
    //                recipientTypeId: { isValid: true },
    //                recipientIds: { isValid: true },
    //                remark: { isValid: true },
    //                chatGroupTypeId: { isValid: false }
    //            };

    //            // Act
    //            let result = updatePopup.checkValidParams();

    //            // Assert
    //            expect(result).toBe(false);
    //        });
    //    });

    //    describe('onChange', () => {
    //        it('when called - setState', () => {
    //            // Arrange
    //            let props = {
    //                ...defaultProps
    //            };
    //            let updatePopup = new EditPopup(props);
    //            updatePopup.setState = jest.fn();
    //            let fieldName = 'field name';
    //            let fieldValue = 123;

    //            // Act
    //            updatePopup.onChange(fieldName, fieldValue);

    //            // Assert
    //            expect(updatePopup.setState).toHaveBeenCalledWith({ [fieldName]: fieldValue, isChanged: true });
    //        });
    //    });

    //    describe('closePopupOnSuccess', () => {
    //        it('call closeUpdatePopupHandler after 1000ms', (done) => {
    //            // Arrange
    //            let props = {
    //                ...defaultProps,
    //                closeUpdatePopupHandler: jest.fn()
    //            };
    //            let updatePopup = new EditPopup(props);

    //            // Act
    //            updatePopup.closePopupOnSuccess();

    //            // Assert
    //            setTimeout(() => {
    //                expect(props.closeUpdatePopupHandler).toHaveBeenCalled();
    //                done();
    //            }, 1000);
    //        });
    //    });

    describe('filterRecipients', () => {
        it('should call correctly functions', () => {
            // Arrange
            const props = {
                settings: {
                    recipients: [
                        { recipientTypeId: 1, id: 1, name: '123bet' },
                        { recipientTypeId: 2, id: 2, name: '365online' }
                    ]
                },
                filterRecipients: jest.fn()
            };
            const container = shallow(<EditPopup {...props} />);
            container.setState = jest.fn();
            const recipientTypeId = 1;

            // Act
            container.instance().filterRecipients(recipientTypeId);

            // Assert
            expect(props.filterRecipients).toHaveBeenCalledWith({ recipients: props.settings.recipients, recipientTypeId });
            expect(container.setState).toHaveBeenCalledWith({ recipientIds: { value: '', isValid: false } });
        });
    });

    //    describe('render', () => {
    //        it('when isShowed is false - return null', () => {
    //            // Arrange
    //            let props = {
    //                updatePopupData: {
    //                    ...defaultProps.updatePopupData,
    //                    isShowed: false
    //                }
    //            };
    //            let updatePopup = new EditPopup(props);

    //            // Act
    //            let result = updatePopup.render();

    //            // Assert
    //            expect(result).toBe(null);
    //        });

    //        it('when filteredRecipients is null - return null', () => {
    //            // Arrange
    //            let props = {
    //                updatePopupData: {
    //                    ...defaultProps.updatePopupData,
    //                    isShowed: true
    //                },
    //                filteredRecipients: null
    //            };
    //            let updatePopup = new EditPopup(props);

    //            // Act
    //            let result = updatePopup.render();

    //            // Assert
    //            expect(result).toBe(null);
    //        });

    //        it('when SuccessMsg shown', () => {
    //            // Arrange
    //            let props = {
    //                updatePopupData: {
    //                    ...defaultProps.updatePopupData,
    //                    isShowed: true
    //                },
    //                filteredRecipients: [],
    //                settingData: {
    //                    recipientTypes: []
    //                },
    //                updateData: { addedresult: true }
    //            };

    //            // Act
    //            let result = (new ShallowRenderer()).render(<EditPopup {...props} />);

    //            // Assert
    //            expect(result).toMatchSnapshot();
    //        });

    //        it('when SuccessMsg is not shown', () => {
    //            // Arrange
    //            let props = {
    //                updatePopupData: {
    //                    ...defaultProps.updatePopupData,
    //                    isShowed: true
    //                },
    //                filteredRecipients: [],
    //                settingData: {
    //                    recipientTypes: []
    //                },
    //                updateData: { addedresult: false }
    //            };

    //            // Act
    //            let result = (new ShallowRenderer()).render(<EditPopup {...props} />);

    //            // Assert
    //            expect(result).toMatchSnapshot();
    //        });
    //    });
});