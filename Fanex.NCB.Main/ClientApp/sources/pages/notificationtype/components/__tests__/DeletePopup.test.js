import React from 'react';
import { DeletePopup } from '../DeletePopup';
import { toast } from 'react-toastify';
import { shallow } from 'enzyme';

describe('DeletePopup', () => {

    afterAll(() => {
        jest.clearAllMocks();
    });

    const initProps = {
        deletePopupData: {
            notificationTypeId: 1,
            successMsg: 'successfully'
        },
        deleteResult: {
            isSuccessful: true,
            message: 'successful'
        }
    };
    
    describe('confirmHandler', () => {
        it('should setState and call exactly props function', async () => {
            // Arrange
            const promise = Promise.resolve({}); 
            const props = {
                ...initProps,
                deleteNotificationType: jest.fn(() => promise)
            };

            // Act
            const container = shallow(<DeletePopup {...props}/>);
            await container.instance().confirmHandler();

            // Assert
            expect(container.state().isLoading).toBe(true);
            expect(props.deleteNotificationType).toHaveBeenCalled();
        });
    });

    describe('closePopup', () => {
        it('should call exactly functions when delete successful', async () => {
            // Arrange
            const props = {
                ...initProps,
                onClose: jest.fn()
            };
            toast.success = jest.fn();

            // Act
            const container = shallow(<DeletePopup {...props}/>);
            await container.instance().closePopup();

            // Assert
            expect(toast.success).toHaveBeenCalledWith('successfully');
            expect(props.onClose).toHaveBeenCalled();
        });

        it('should call exactly functions when delete unSuccessful', async () => {
            // Arrange
            const props = {
                ...initProps,
                deleteResult: {
                    isSuccessful: false,
                    message: 'false delete'
                },
                onClose: jest.fn()
            };
            toast.error = jest.fn();

            // Act
            const container = shallow(<DeletePopup {...props}/>);
            await container.instance().closePopup();

            // Assert
            expect(toast.error).toHaveBeenCalledWith('false delete');
            expect(props.onClose).toHaveBeenCalled();
        });
    });

    describe('render', () => {
        it('should call exactly function when click button', () => {
            // Arrange
            const props = {...initProps};
            const confirmHandler = jest.fn();

            // Act
            const container = shallow(<DeletePopup {...props}/>);
            container.instance().confirmHandler = confirmHandler;
            const render = shallow(container.instance().render());

            render.find('button').at(0).simulate('click');

            // Assert
            expect(confirmHandler).toHaveBeenCalled();
        });
    });
});