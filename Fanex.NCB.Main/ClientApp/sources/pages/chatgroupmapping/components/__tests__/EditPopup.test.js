import React from 'react';
import { EditPopup } from '../EditPopup';
import { shallow } from 'enzyme';
//import { getLicenseeStatus, isMapped } from '../../../common/CommonFunctions';
//import CommonFunctions from '../../../common/common-functions/CommonFunctions';

describe('EditPopup', () => {
    afterAll(() => {
        jest.clearAllMocks();
    });

    //    const defaultProps = {
    //        updatePopupData: {
    //            editingItem: {
    //                groupID: 1,
    //                conversationID: 2,
    //                recipientTypeId: { value: 3 },
    //                chatGroupTypeId: { value: 1 },
    //                recipientIds: { value: '4,5' },
    //                remark: { value: 'remark 1' },
    //                mappingStatusId: 1
    //            }
    //        }
    //    };

    //    describe('constructor', () => {
    //        it('get state from prop', () => {
    //            // Arrange
    //            let props = {
    //                ...defaultProps
    //            };

    //            // Act
    //            let updatePopup = new EditPopup(props);

    //            // Assert
    //            expect(updatePopup.state.groupID).toEqual(props.updatePopupData.editingItem.groupID);
    //            expect(updatePopup.state.conversationID).toEqual(props.updatePopupData.editingItem.conversationID);
    //            expect(updatePopup.state.recipientTypeId).toEqual(props.updatePopupData.editingItem.recipientTypeId);
    //            expect(updatePopup.state.recipientIds).toEqual(props.updatePopupData.editingItem.recipientIds);
    //            expect(updatePopup.state.remark).toEqual(props.updatePopupData.editingItem.remark);
    //            expect(updatePopup.state.isSummited).toEqual(false);
    //        });
    //    });

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

    //    describe('submitHandler', () => {
    //        it('when not valid params - not call update', () => {
    //            // Arrange
    //            let props = {
    //                ...defaultProps
    //            };
    //            let updatePopup = new EditPopup(props);
    //            let event = { preventDefault: jest.fn() };
    //            updatePopup.setState = jest.fn();
    //            updatePopup.checkValidParams = jest.fn(() => { return false; });
    //            updatePopup.state.isSummited = false;

    //            // Act
    //            updatePopup.submitHandler(event);

    //            // Assert
    //            expect(event.preventDefault).toHaveBeenCalled();
    //            expect(updatePopup.setState).toHaveBeenCalledWith({ isSummited: true });
    //        });

    //        it('when valid params and updateData isSuccessful - call update', (done) => {
    //            // Arrange
    //            let response = {};
    //            let props = {
    //                ...defaultProps,
    //                update: jest.fn(() => Promise.resolve(response)),
    //                updateData: { isSuccessful: true }
    //            };
    //            let updatePopup = new EditPopup(props);
    //            let event = { preventDefault: jest.fn() };
    //            updatePopup.setState = jest.fn();
    //            updatePopup.checkValidParams = jest.fn(() => { return true; });
    //            updatePopup.state.isSummited = true;
    //            let params = 'params';
    //            updatePopup.getParams = jest.fn(() => { return params; });
    //            updatePopup.closePopupOnSuccess = jest.fn();

    //            let isMapped = jest.fn(() => { return false; });
    //            CommonFunctions.isMapped = isMapped.bind(CommonFunctions);

    //            // Act
    //            updatePopup.submitHandler(event);

    //            // Assert
    //            expect(event.preventDefault).toHaveBeenCalled();
    //            expect(updatePopup.setState).not.toHaveBeenCalled();
    //            expect(props.update).toHaveBeenCalledWith(params);
    //            props.update().then(() => {
    //                expect(updatePopup.closePopupOnSuccess).toHaveBeenCalled();
    //                done();
    //            });
    //        });

    //        it('when valid params and updateData errorCode is NOT 0 - do not call closePopup', (done) => {
    //            // Arrange
    //            let response = {};
    //            let props = {
    //                ...defaultProps,
    //                update: jest.fn(() => Promise.resolve(response)),
    //                updateData: { isSuccessful: false }
    //            };
    //            let updatePopup = new EditPopup(props);
    //            let event = { preventDefault: jest.fn() };
    //            updatePopup.setState = jest.fn();
    //            updatePopup.checkValidParams = jest.fn(() => { return true; });
    //            updatePopup.state.isSummited = true;
    //            let params = 'params';
    //            updatePopup.getParams = jest.fn(() => { return params; });
    //            updatePopup.closePopup = jest.fn();

    //            // Act
    //            updatePopup.submitHandler(event);

    //            // Assert
    //            expect(event.preventDefault).toHaveBeenCalled();
    //            expect(updatePopup.setState).not.toHaveBeenCalled();
    //            expect(props.update).toHaveBeenCalledWith(params);
    //            props.update().then(() => {
    //                expect(updatePopup.closePopup).not.toHaveBeenCalled();
    //                done();
    //            });
    //        });
    //    });

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
});